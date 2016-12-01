# Unity Learning Environment
--------------------------------

Unity Learning Environment (ULE) is a framework for Reinforcement Learning for Unity3D and Python.
The software is under development and should in its current state be considered Alpha quality at best.

![alt tag](https://raw.githubusercontent.com/badgeir/ule/master/doc/pong.png)

### Prerequisites
ULE requires an installation of Unity 5 and a scientific Python distribution with numpy and matplotlib installed in order to run.

### Setting up ULE

Clone or download the repository.

Open a command prompt in the repository root, and run

	python setup.py install

## Usage
----------------
### Running an example scene
Open up the example scene *pong.unity*, and press play.
Open up an interactive python shell and import ule:

	>python
	>>>import ule

To connect to the running scene in Unity, write
	
	>>>env = ule.load()

This will connect to the server running in Unity, and get the environment information for the running scene.
The main components of the ule interface are *sensors* and *motors*. Every available sensor and motor in the
Unity scene are available through the ule environment's lists of sensors and motors:

	>>>env.sensors
	[Ball: Vector3 Sensor, Camera: Image(84L, 84L, 3L) Sensor]
	>>>env.motors
	[Paddle: Discrete(3) Motor]

This scene holds two sensors, an Image sensor with the name 'Camera' and a Vector sensor called 'Ball'. It also holds a motor named 'Paddle'. For those familiar with openai gym, sensors and motors are in fact abstractions of gym's *spaces*, that define the valid actions and observations for the motors and sensors. The Vector space defines a float vector of length N (where N = 3 for the Ball sensor), and the Image space is really a renamed version of gym's Box. In ULE, the intention is not to work directly on those spaces, but rather through the sensor and motor abstractions. Each sensor and motor holds a name and a value, available through getter and setter methods.
	
	>>> env.sensors[0].name()
	'Ball'
	>>>ball = env.sensors[0]
	>>>ball.value()
	array([ 0.,  0.,  0.  ])
	>>>
	>>>paddle = env.motors[0]
	>>>paddle.value()
	0
	>>>paddle.set_value(1)
	>>>paddle.value()
	1

The motor class also wraps the spaces *contains* method, which checks if the value is valid in the given motor space. The Paddle motors Discrete(3) space has a valid range of the integers 0 - 2.

	>>>paddle.contains(2)
	True
	>>>paddle.contains(3)
	False

In order to send motor output and receive sensor feedback from the Unity scene,
*env.step()* is used, much like in openai gym. In contrast to gym, the step() function takes no input, but rather
sends the member motor values when sending to Unity.

	>>>reward, done, info = env.step()
	>>>ball.value()
	array([ 0.01,  0.01,  0.  ])

Here, the motor value of '1' was sent to the Paddle motor in the Unity scene, and the Ball sensor has been updated one timestep with the new Ball sensor value.

The Image sensor, like the Vector sensor, holds a numpy array. To view the image from the Camera sensor, one can use pyplot:
	
	>>>camera = env.sensors[1]
	>>>from matplotlib import pyplot as plt
	>>>plt.imshow(camera.value())
	>>>plt.show()

![alt tag](https://raw.githubusercontent.com/badgeir/ule/master/doc/imshow.png)

Finally, to close a connection to the environment:

	>>>env.close()

### An example agent

The following script plays five consecutive games, and outputs a random motor value at each step:

	import numpy as np
	import ule
	
	def main():
	    env = ule.load()

	    for game in range(5):
	        done = 0
	        total_reward = 0
	        while done == 0:
	            for motor in env.motors:
	                motor.randomize()
	
	            reward, done, info = env.step()
	            
	            total_reward += reward
	
	        print('game %d, total reward: %f\n'%(game, total_reward))
	        
	        env.reset()
	
	    env.close()
	
	if __name__ == '__main__':
	    main()
	

## Setting up a new scene

In this section, we will build up a new scene from scratch. Basic knowledge of the Unity game engine is assumed.

We will make a very simple scene where we control a cube with a discrete motor which controls three available actions: 
stand still, move up and move down. 
A vector sensor will read back it's current height. We will add a reward every time the height reaches above
a multiple of 1, and the game ends either when the height reaches a lower threshold -10 (losing condition),
or a high threshold 10 (victory condition).

In Unity, create a new scene and make sure you have the unityproj/Assets/ULE directory imported in your projected, either by opening the example project (unityproj),
or by copying it into the Assets folder in your project.

Create a new Cube shape. The cube will have a Vector Sensor and a Discrete Motor attached to it. In the Inspector menu, with the Cube selected, add a new C# script, and name it HeightSensor.

In the folder ULE/Sensor/Templates you will find a template script, *VectorSensorTemplate.cs*. Copy the contents of this script to HeightSensor.cs, and rename the class to HeightSensor. HeightSensor.cs should now look like this:

	using UnityEngine;
	using System.Collections;
	using ULE;

	public class HeightSensor : VectorSensor
	{
	
		void Start()
		{
			mName = "Name";
			mLength = 3;
			mMinVal = -100;
			mMaxVal = 100;
	
			//remember to initialize base class
			base.Init();
		}
	
		protected override void Sample()
		{
			mVector[0] = transform.position.x;
			mVector[1] = transform.position.y;
			mVector[2] = transform.position.z;
		}
	}

We only need one value to describe the height, so change the length of the vector to 1. Also, change the
name to "Height". The min and max values are there so that it is simpler to normalize the input during training.
We can let those stay at the current values. The Start() function should now look like this:

	void Start()
	{
		mName = "Height";
		mLength = 1;
		mMinVal = -100;
		mMaxVal = 100;

		//remember to initialize base class
		base.Init();
	}

The Sample function is called each time we make a step() from python. In here, we set the sensor values which will be sent back to the python sensor.
We will set the value of the first (and only) vector element so that it contains the height of the cube.
Change the Sample function to the following:

	protected override void Sample()
	{
		mVector[0] = transform.position.y;
	}

That's it for the Height sensor. Let's move on to the motor.

Add a new Script, 'HeightMotor' to the cube.
This time, copy the content from ULE/Sensor/Templates/DiscreteMotorTemplate.cs (and remember to change the class name). We need three actions, so mNumActions should be set to 3.

In the Act() method, an integer is given each time we update the motors from python using step().
Here, we edit the switch statement for out purposes (Do nothing, move up and move down).

The HeighMotor.cs script should look like this when finished:

	using UnityEngine;
	using System.Collections;
	using ULE;
	
	public class HeightMotor : DiscreteActionMotor
	{
	
		void Start()
		{
			mName = "HeightMotor";
			mNumActions = 3;
	
			// remember to init base class
			base.Init();
		}
	
		// Called when new action is received
		protected override void Act(int action)
		{
			switch (action)
			{
				case 0: break; // do nothing
				case 1: transform.Translate(0, 0.1f, 0); break; // move up
				case 2: transform.Translate(0, -0.1f, 0); break; // move down
				default: break;
			}
		}
	}

Our sensors and motors are now done. We now have to add another script for taking care of the
communication with python. Add an empty object and name it 'Agent' (The name is important, as the sensors and motors uses the name to automatically attach themselves to the Agents list of sensors and motors. Attach the *ReinforcementAgent* script to Agent. The ReinforcementAgent runs a TCP server, and takes care of the communication between the Unity scene and out python environment. All sensors and motors in the scene are automatically kept track of by the ReinforcementAgent. The hierarchy should now look like the following,
where 'Cube' has the HeighSensor and HeightMotor scripts attached, and 'Agent' has the ReinforcementAgent attached.

![alt tag](https://raw.githubusercontent.com/badgeir/ule/master/doc/hierarchy.png)

Let's take a look in python to make sure that the sensors and motors show up. Press play in the Unity editor
to start the scene. Then open up a python console, and start a ule environment:

	>python
	>>>import ule
	>>>env = ule.load()
	>>>env.sensors
	[Height: Vector1 Sensor]
	>>>env.motors
	[HeightMotor: Discrete(3) Motor]

Allright, all good so far. Let's set our motor output to 1, and make a single step, to make sure our sensor
updates with the height measurement:

	>>>env.sensors[0].value()
	array([ 0.])
	>>>env.motors[0].set_value(1)
	>>>env.step()
	>>>env.sensors[0].value()
	array([ 0.1])

Good.