# Unity Learning Environment
--------------------------------

Unity Learning Environment (ULE) is a lightweight framework for Reinforcement Learning for Unity3D and Python.
The software is under development and should in its current state be considered Alpha quality at best.

![alt tag](https://raw.githubusercontent.com/badgeir/ule/master/pong.png)

### Prerequisites
ULE requires an installation of Unity 5 and a Python distribution with numpy installed in order to run.

### Setting up ULE

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
Unity scene are available through the environments sensors() and motors() getters, which return Lists containing
the scene's sensors and motors.

	>>>sensors = env.sensors()
	>>>motors = env.motors()
	>>>sensors
	[Ball: Vector3 Sensor, Camera: Image(84L, 84L, 3L) Sensor]
	>>>motors
	[Paddle: Discrete(3) Motor]

This scene holds two sensors, an Image sensor with the name 'Camera' and a Vector sensor called 'Ball'. It also holds a motor named 'Paddle'. For those familiar with openai gym, sensors and motors are in fact abstractions of gym's *spaces*, that define the valid actions and observations for the motors and sensors. The Vector space defines a float vector of length N (where N = 3 for the Ball sensor), and the Image space is really a renamed version of gym's Box. In ULE, the intention is not to work directly on those spaces, but rather through the sensor and motor abstractions. Each sensor and motor holds a name and a value, available through getter and setter methods.
	
	>>> sensors[0].name()
	'Ball'
	>>>ball = sensors[0]
	>>>ball.value()
	array([ 0.01,  0.01,  0.  ])
	>>>
	>>>paddle = motors[0]
	>>>paddle.value()
	0
	>>>paddle.set_value(1)
	>>>paddle.value(1)
	1

The motor class also wraps the spaces *contains* method, which checks if the value is valid in the given motor space. The Paddle motors Discrete(3) space has a valid range of the integers 0 - 2.

	>>>paddle.contains(1)
	True
	>>>paddle.contains(3)
	False

In order to send motor output and receive sensor feedback from the Unity scene,
*env.step()* is used, much like in openai gym. In contrast to gym, the step() function takes no input, but rather
sends the member motor values when sending to Unity.

	>>>reward, done, info = env.step()
	>>>ball.value()
	array([ 0.02,  0.02,  0.  ])

Here, the motor value of '1' was sent to the Paddle motor in the Unity scene, and the Ball sensor has been updated one timestep with the new Ball sensor value.

The Image sensor, like the Vector sensor, holds a numpy array. To view the image from the Camera sensor, one can use pyplot:
	
	>>>camera = sensors[1]
	>>>from matplotlib import pyplot as plt
	>>>plt.imshow(camera.value())
	>>>plt.show()

![alt tag](https://raw.githubusercontent.com/badgeir/ule/master/imshow.png)

Finally, to close a connection to the environment:

	>>>env.close()

### An example agent

The following script plays five consecutive games, and outputs a random motor value at each step:

	import numpy as np
	import ule
	
	def main():
	    env = ule.load()
	    sensors = env.sensors()
	    motors = env.motors()
	
	    for game in range(5):
	        done = 0
	        total_reward = 0
	        while done == 0:
	            for motor in motors:
	                motor.randomize()
	
	            reward, done, info = env.step()
	            
	            total_reward += reward
	
	        print('game %d, total reward: %f\n'%(game, total_reward))
	        
	        env.reset()
	
	    env.close()
	
	if __name__ == '__main__':
	    main()
	

### Setting up a new scene