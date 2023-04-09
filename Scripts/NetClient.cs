using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class NetClient : MonoBehaviour
{
    Socket server, client;
    Thread thread;
    public int flag = -3;
    public bool isconnect = false, resvflag = false;
    public string servername = null, data = null;
    public GameObject startui;
    public Player player;
    public NetManager netmanager;
    byte[] result = new byte[256];
    public void Start(){
        //player = GameObject.Find("Player").GetComponent<Player>();
        client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        thread = new Thread(waite);
    }
    private void FixedUpdate() {
        work();
    }

    public void connectserver(string ip, int port){
        Debug.Log(string.Format("sent ask to {0} : {1}", ip, port.ToString()));
        try{
            client.Connect(IPAddress.Parse(ip), port);
            if (client.Connected){
                isconnect = true;
            flag = 0;
            thread.Start();
            sendmessage(netmanager.localname);
            }
        }
        catch (Exception ex){
            Debug.Log(ex.Message);
            isconnect = false;
        }
    }

    public void sendmessage(string data){
        Debug.Log(string.Format("send msg: {0}", data));
        if (client.Connected == false || data.Length == 0)
            return;
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        client.Send(buffer);
    }

    public void closeclient(){
        isconnect = false;
        try{client.Shutdown(SocketShutdown.Both);}catch{}
        client.Close();
        thread.Abort();
        Debug.Log("clientwork close");
        if (!startui.transform.Find("startUI").gameObject.activeSelf && flag != -3)
            startui.GetComponent<StartUI>().gamereturn();
        flag = -3;
    }

    public void work(){
        if (flag == -1){
            closeclient();
            flag = -3;
            return;
        }
        if (!resvflag){
            return;
        }
        resvflag = false;
        Debug.Log(flag);

        if (flag == 0){//data解释为name
            servername = data;
            flag = 1;
        }
        else if (flag == 1){//仍未联机 等待联机消息
            if (string.Compare(data, "allow") == 0){
                Debug.Log("联机成功");
                flag = 2;
                startui.GetComponent<StartUI>().startgame(false);
            }

        }
        else if (flag == 2){//联机中 解释为同步信息
            startui.GetComponent<StartUI>().player.setmsg(data);
        }
        data = null;
    }

    public void setwork(string data){
        resvflag = true;
        this.data = data;
    }

    public void waite(){
        while(isconnect){
            try{
                int num = client.Receive(result);
                if (num == 0){continue;}
                string data = Encoding.UTF8.GetString(result,0,num);
                Debug.Log(string.Format("{0}: {1}b\n{2}", client.RemoteEndPoint.ToString(), num.ToString(), data));
                //Debug.Log(string.Compare(data, "allow") == 0);
                setwork(data);
            }
            catch ( Exception ex){
                Debug.Log(ex.Message);
                flag = -1;
                return;
                //closeclient();
            }
        }
    }

    private void OnDestroy() {
        isconnect = false;
        flag = -1;
        thread.Abort();
    }
    private void OnApplicationQuit() {
        flag = -1;
        thread.Abort();
    }
}

class pack{
    int flag;
    object obj;
    pack(int flag, object obj){
        this.flag = flag;
        this.obj = obj;
    }
}
