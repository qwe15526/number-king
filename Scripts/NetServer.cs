using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using UnityEngine;
using System.Text;

public class GeIP
{
    public enum address
    {
        IPv4, IPv6
    }
     /// <summary>
     /// 获取本机IP
     /// </summary>
     /// <param name="Addfam">要获取的IP类型</param>
     /// <returns></returns>
     public static string IP(address fam)
     {
         if (fam== address .IPv6 && !Socket.OSSupportsIPv6)
         {
             return null;
         }
         string output = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
             NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
             NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
 
             if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
             {
                 foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                 {
                     if (fam== address.IPv4)
                     {
                         if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                         {
                             output = ip.Address.ToString();
                         }
                     }
                     else if (fam== address.IPv6)
                     {
                         if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                         {
                             output = ip.Address.ToString();
                         }
                     }
                 }
             }
        }
         return output;
     }
}


public class NetServer : MonoBehaviour
{
    Socket server;
    public List<Clientwork> clients = new List<Clientwork>();
    public Clientwork selectedclient = null;
    public string selectclientname = null;
    Thread connectthread = null;
    public string ip = "null";
    public int port = 12312;
    public bool isthreadrun = false;
    byte[] result = new byte[1024];
    public NetManager netmanager;
    public Player player;
    public StartUI startui;
    Action<string,string> act;
    public void Start(){
        ip = GeIP.IP(GeIP.address.IPv6);
        act = new Action<string, string>(this.startui.setlinkask);
        //player = GameObject.Find("Player").GetComponent<Player>();
        server = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

    }

    void Update(){
        if (clients.Count == 0){
            return;
        }else{
            foreach( Clientwork clientwork in clients){
                clientwork.work();
            }
        }
    }

    public void runserver(){
        IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
        try{server.Bind(point);server.Listen(5); ;}catch(Exception ex){Debug.Log(ex.Message);}  //开始监听来自其他计算机的连接
        Debug.Log("start listen");
        connectthread = new Thread(waitconnect);
        isthreadrun = true;
        connectthread.Start();
    }

    public void closesocket(Socket s){
        try{
            s.Dispose();
            s.Shutdown(SocketShutdown.Both);
        }catch(Exception ex){Debug.Log(ex.Message);}
        try{
            s.Close();
        }catch(Exception ex){Debug.Log(ex.Message);}
    }

    public void closeclients(){
        if (clients.Count == 0)
            return;
        foreach (Clientwork clientwork in clients){
            clientwork.closeclient();
        }
        clients.Clear();
    }
    public void closeclientbyname(string name){
        Clientwork clientwork = clients.Find((Clientwork s) => { return s.clientname == name; } );
        if (clientwork != null)
            clientwork.closeclient();
    }

    public void stopserver(){
        isthreadrun = false;
        closeclients();
        this.closesocket(server);
    }

    public void setport(string port){
        this.port = int.Parse(port);
    }

    public void waitconnect(){
        while(isthreadrun){
            Socket client = server.Accept();
            clients.Add(new Clientwork(this, client));
        }
    }

    public void startgame(){
        if ( selectedclient != null){
            netmanager.GetComponent<NetManager>().isserver = true;
            selectedclient.flag = 2;
            sendseletedmsg("allow");
        }
    }
    public void selectclient(Clientwork clientwork){
        //selectedclient = clients.Find( s => string.CompareOrdinal(s.clientname, clientname) == 0);
        selectedclient = clients.Find( s => s == clientwork);
        //selectclientname = this.selectedclient.clientname;
        Debug.Log(selectedclient.clientname);
        startui.starttogamebutton.GetComponent<UnityEngine.UI.Button>().enabled = true;
    }

    public void sendseletedmsg(string data){
        sendmessage(data, selectedclient.client);
    }
    public void sendmessage(string data, Socket client){
        Debug.Log(string.Format("{0}send msg :{1}", client.RemoteEndPoint, data));
        if (client == null || data.Length == 0){
            return;}
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        client.Send(buffer);
    }
    private void OnDestroy() {
        isthreadrun = false;
        closeclients();
    }
    private void OnApplicationQuit() {
        isthreadrun = false;
        closeclients();
    }
}

public class Clientwork
{
    public Socket client = null;
    public NetServer server = null;
    public string clientname = "";
    public string data;
    Thread thread = null;
    public bool resvflag = false;
    byte[] result = new byte[256];
    public int flag = -1;//默认-1， 游戏开始2

    public Clientwork(NetServer server, Socket client){
        flag = 0;
        this.client = client;
        this.server = server;
        this.thread = new Thread(waite);
        this.thread.Start();
    }
    public void setwork(string data){
        this.data = data;
        this.resvflag = true;
    }

    public void work(){
        if (flag == -1){
            closeclient();
            server.startui.gamereturn();
            flag = -3;
        }
        if (!resvflag){
            return;
        }
        resvflag = false;

        if (flag == 0){//data解释为name
            clientname = data;
            server.startui.addlinkclient(this);
            server.sendmessage(server.netmanager.localname, client);
            flag = 1;
        }
        else if (flag == 1){//仍未联机 忽略信息

        }
        else if (flag == 2){//联机中 解释为同步信息
            server.startui.player.setmsg(data);
        }
    }

    public void waite(){
        while(flag != -1){
            try{
                int num = this.client.Receive(result);
                if (num == 0){continue;}
                string data = Encoding.UTF8.GetString(result,0,num);
                Debug.Log(string.Format("{0}: {1}b\n{2}", client.RemoteEndPoint.ToString(), num.ToString(), data));
                setwork(data);//非线程安全 之后修改
            }
            catch ( Exception ex){
                Debug.Log(ex.Message);
                flag = -1;
                //closeclient();
                return;
            }
        }
        Debug.Log("thread break");
    }

    public void closeclient(){
        flag = -3;
        thread.Abort();
        Debug.Log("clientwork close");
        server.clients.Remove(this);
        server.startui.dellinkclient(this);
    }

    ~Clientwork(){
        closeclient();
    }
}
