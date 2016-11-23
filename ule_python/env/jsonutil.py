
import json

def decodeJsonStr(str):
    jsn = json.loads(str)
    return decodeJson(jsn)

def decodeJson(info):
    decoded = {}
    try:
        for key in info.keys():
            k = str(key)
            if info[k].has_key('type'):
                dtype = str(info[k]['type'])
                if dtype == 'int':
                    decoded[k] = int(info[k]['value'])
                elif dtype == 'float' or dtype == 'System.Single':
                    decoded[k] = float(info[k]['value'])
            elif k == 'observation':
                decoded[k] = decodeJson(info[k])
    
    except Exception as e:
        print('Error decoding json string:', e)
    return decoded