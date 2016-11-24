
import ule

env = ule.load()
s = env.sensors()
m = env.motors()

print(s[0])
print(m[0])