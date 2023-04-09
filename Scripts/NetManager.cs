using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    public Int32 _lock;
    public string localname = "new name";
    Queue<Action> _wait = new Queue<Action>();
    public bool _run = false, isserver=false;
    // Start is called before the first frame update
    void Start()
    {
        localname = "new name";
    }

    // Update is called once per frame
    void Update()
    {
        if (_run){
            Queue<Action> execute = null;
            if (0 == Interlocked.Exchange(ref _lock, 1)){
                execute = new Queue<Action>(_wait.Count);
                while ( _wait.Count != 0){
                    Action action = _wait.Dequeue();
                    execute.Enqueue(action);
                }
                _run = false;
                Interlocked.Exchange(ref _lock, 0);
            }
            if (execute != null){
                while (execute.Count != 0){
                    Action action = execute.Dequeue();
                    action();
                }
            }
        }
    }

    public void begininvoke(System.Action action){
        while(true){
            if (0 == Interlocked.Exchange(ref _lock, 1)){
                _wait.Enqueue(action);
                _run = true;
                Interlocked.Exchange(ref _lock, 0);
                break;
            }
        }
    }

    public void sendmsg(string data){//区分 服务端和客户端 发送消息
        if (isserver){
            this.gameObject.GetComponent<NetServer>().sendseletedmsg(data);
        }
        else{
            this.gameObject.GetComponent<NetClient>().sendmessage(data);
        }
    }
    public void closelink(){
        if (isserver){
            this.GetComponent<NetServer>().closeclients();
        }
        else{
            this.GetComponent<NetClient>().closeclient();
        }
    }

    public void setplayer(Player player){
        this.GetComponent<NetServer>().player = player;
        this.GetComponent<NetClient>().player = player;
    }
}

