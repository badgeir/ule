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


        mRunMode = RunMode.Discrete;

        if(mRunMode == RunMode.Discrete)
        {
            Time.timeScale = 0;
        }

        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mSensors = new List<Sensor>();
        mMotors = new List<Motor>();

        mUleRPC = new UleRPC(OnGetEnvironment, OnUpdateMotor, Reset);

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
            // parse message from client
            mUleRPC.ParseInput(mPendingMessages.Dequeue());

            if (mMotorsUpdated)
            {
                // if runmode is discrete, step environment one physics step
                if(mRunMode == RunMode.Discrete)
                {
                    Tick();
                }

                StartCoroutine(SendFeedback());

                mAccumulatedReward = 0;
                mMotorsUpdated = false;
            }

        }
    }

    IEnumerator SendFeedback()
    {
        if(mRunMode == RunMode.Discrete)
        {
            while(Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.001f);
            }
        }
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
    }

    void Tick()
    {
        StartCoroutine(TickPhysics());
    }

    IEnumerator TickPhysics()
    {
        Time.timeScale = 1;
        yield return new WaitForFixedUpdate();
        Time.timeScale = 0;
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
        mPendingMessages.Enqueue(msg);
    }

    /***************************************************
     * UleRPC Callback functions
     **************************************************/
    public void OnGetEnvironment()
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

    public void OnUpdateMotor(JSONNode motor)
    {
        foreach (Motor m in mMotors)
        {
            string name = motor["name"];
            if (m.name() == name)
            {
                mMotorsUpdated = true;
                m.PushJson(motor);
            }
        }
    }

    public void Reset()
    {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mMotorsUpdated = false;

        mSensors.Clear();
        mMotors.Clear();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        SendFeedback();
    }

}
