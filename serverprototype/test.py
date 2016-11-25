
import socket
import time

hostIP = "127.0.0.1"
port = 3000

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((hostIP, port))

sock.send("Hello!")
response = sock.recv(1024)

print(response)
time.sleep(5)

sock.close()