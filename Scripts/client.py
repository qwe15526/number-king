import socket, json, time;

if input() == "1":
    client = socket.socket(socket.AF_INET6, socket.SOCK_STREAM)
    client.settimeout(100)
    host = socket.gethostname()
    port = 12312
    name = "qwe"
    data = ""

    try:
        err = client.connect((host,port))
        print("ask...")
        time.sleep(3)
        client.send(name.encode())
        print("send name...")
        data = str(client.recv(1024), encoding="utf-8")
        print("recerve data:", data)
    except:
        print(err, data)
        print("link is closed")
        client.close()
        exit()

    if len(data) == 0:
        client.close()
        exit()

    severname = data
    data = ''

else:
    server = socket.socket(socket.AF_INET6, socket.SOCK_STREAM)
    #server.settimeout(-1)
    host = socket.gethostname()
    port = 12345
    name = "qwe"
    data = ""

    server.bind((host, port))
    server.listen(5)
    server.settimeout(100000)
    print("start server at", host, port)
    client = None
    while True:
        client, adds = server.accept()
        print(adds)
        if client:
            break
    clientname = client.recv(1024)
    print(str(clientname, encoding="utf-8"))
    client.send(name.encode())
    time.sleep(1)
    client.send("allow".encode())

while True:
    try:
        data = client.recv(1024)
        if len(data) == 0:
            continue
        s = input("send data").split(",")
        data = {"flag":int(s[0]), "cardtype":int(s[1]), "cardposition":[int(s[2]), int(s[3])], "toposition":[int(s[4]), int(s[5])]}
        print(data)
        client.send((json.dumps(data)).encode())
        data = ''
    except:
        client.close()
        print("link is closed")
        exit()
