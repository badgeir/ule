
import socket
import time

import json

hostIP = "127.0.0.1"
port = 3000

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((hostIP, port))

jmsg = {}

jmsg['method'] = 'updateEnvironment'
jmsg['parameters'] = {}
jmsg['parameters']['motors'] = []

motor1 = {}
motor1['name'] = 'motor1'
motor1['value'] = [1,2,3]
jmsg['parameters']['motors'].append(motor1)

msg = json.dumps(jmsg)

sock.send(msg)
response = sock.recv(1024)

print(response)

sock.close()