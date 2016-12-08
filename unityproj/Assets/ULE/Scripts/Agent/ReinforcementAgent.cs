using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

using ULE;

public class ReinforcementAgent : MonoBehaviour {

    enum RunMode { Discrete, Continuous }
    RunMode mRunMode;

    List<Sensor> mSensors;
    List<Motor> mMotors;

    enum GameStatus { StatusOK, GameOver };
    GameStatus mGameStatus;

    float mAccumulatedReward;

    Queue<string> mPendingMessages;

    int mPort;
    ULEServer mServer;
    UleRPC mUleRPC;

    bool mMotorsUpdated;

    static bool mCreated = false;
    bool mIsDuplicate;

    void Awake()
    {
        mRunMode = RunMode.Discrete;

        if (mRunMode == RunMode.Discrete)
        {
            Time.timeScale = 0;
        }

        mPort = 3000;
        string[] args =  System.Environment.GetCommandLineArgs();
        foreach(string arg in args)
        {
            if(arg.Contains("port"))
            {
                try
                {
                    string[] strs = arg.Split('=');
                    mPort = int.Parse(strs[1]);
                    Debug.Log("port = " + mPort);
                }
                catch(System.Exception e)
                {
                    Debug.LogError("Exception: Failed to parse port, using default value of 3000.");
                }
            }
        }

        Application.targetFrameRate = 120;
        //Check if Agent already exists
        if(!mCreated)
        {
            DontDestroyOnLoad(transform);
            mCreated = true;
        }
        else
        {
            mIsDuplicate = true;
            Destroy(gameObject);
        }

        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mSensors = new List<Sensor>();
        mMotors = new List<Motor>();

        mUleRPC = new UleRPC(RPCGetEnvironment, RPCStep, RPCReset, RPCConfig);

        mServer = new ULEServer("127.0.0.1", mPort);
        mServer.Connect(OnClientConnected);

        mPendingMessages = new Queue<string>();

        mMotorsUpdated = false;
    }

    //main loop, called each frame
    void Update()
    {
        if (mPendingMessages.Count > 0)
        {
            // Debug.Log(mPort + " Update");
            // parse message from client
            mUleRPC.ParseInput(mPendingMessages.Dequeue());

            if (mMotorsUpdated)
            {
                // Debug.Log(mPort + " Motors updated");

                // if runmode is discrete, step environment one physics step
                if(mRunMode == RunMode.Discrete)
                {
                    // tick physics one step
                    // timescale is set to zero again after feedback has been sent
                    Time.timeScale = 1;
                }
            }

        }
    }

    void FixedUpdate()
    {
        if(mMotorsUpdated)
        {
            SendFeedback();
        }
        Time.timeScale = 0;
    }

    void SendFeedback()
    {
        // Debug.Log(mPort + " SendFeedback");

        // Optimization. JSONNode.ToString() used 160 ms when parsing image. By writing out the json string
        // "by hand", I got it down to 2 ms.
        string jsonstring = "{{\"sensors\": [{0}], \"reward\": \"{1}\", \"done\": \"{2}\", \"info\": \"{3}\"}}";
        string sensors = "";
        int idx = 0;
        foreach (Sensor sens in mSensors)
        {
            if (idx++ != 0)
            {
                sensors += ", ";
            }
            sensors += sens.SampleJson();
        }
        string info = "";

        string output = string.Format(jsonstring, sensors, mAccumulatedReward, (int)mGameStatus, info);
        mServer.SendResponse(output);

        mAccumulatedReward = 0;
        mMotorsUpdated = false;
    }

    public void AddReward(float reward)
    {
        mAccumulatedReward += reward;
    }

    public void GameOver()
    {
        mGameStatus = GameStatus.GameOver;
    }

    public void OnDestroy()
    {
        if (!mIsDuplicate)
        {
            mServer.Close();
        }
    }

    public void AddSensor(Sensor sensor)
    {
        mSensors.Add(sensor);
    }

    public void AddMotor(Motor motor)
    {
        mMotors.Add(motor);
    }

    /***************************************************
     * Server callback functions 
     **************************************************/
    public void OnClientConnected()
    {
        Debug.Log("Client connected.");
        mServer.StartReceiving(OnClientRequest, OnClientDisconnected);
    }

    public void OnClientDisconnected()
    {
        Debug.Log("Client disconnected.");
        mServer.Connect(OnClientConnected);
    }

    public void OnClientRequest(string msg)
    {
        // Debug.Log(mPort + " OnClientRequest");
        mPendingMessages.Enqueue(msg);
    }

    /***************************************************
     * UleRPC Callback functions
     **************************************************/
    public void RPCGetEnvironment()
    {
        JSONClass json = new JSONClass();

        int idx = 0;
        foreach (Sensor sens in mSensors)
        {
            json["sensors"][idx++] = sens.JsonDescription();
        }

        idx = 0;
        foreach (Motor motor in mMotors)
        {
            json["motors"][idx++] = motor.JsonDescription();
        }

        mServer.SendJson(json);
    }

    public void RPCStep(JSONNode parameters)
    {
        // Debug.Log(mPort + " Step");
        // Update motor outputs
        JSONNode motors = parameters["motors"];
        for (int current = 0; current < motors.Count; current++)
        {
            foreach (Motor m in mMotors)
            {
                string name = motors[current]["name"];
                if (m.name() == name)
                {
                    m.PushJson(motors[current]);
                }
            }
        }
        mMotorsUpdated = true;
    }

    public void RPCReset()
    {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mMotorsUpdated = false;

        mSensors.Clear();
        mMotors.Clear();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        SendFeedback();
    }

    public void RPCConfig(JSONNode config)
    {
        if(config["fps"] != null)
        {
            Application.targetFrameRate = config["fps"].AsInt;
            config.Remove("fps");
        }
    }

}
