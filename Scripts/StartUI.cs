using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    public GameObject startui, rightrect, canvas, maincamera, game,  netmanager, nametext, iptext, porttext, record, deletrecold, serveriplable, severportlable, linkbutton, messagepanel, linkclientcontent, starttogamebutton, bgmusicsroll,effictscrool, datamanager, gamepre;

    public Player player;
    public GameObject buttonpre;
    public GameObject activerect = null;
    public string ip, linkname;

    public bool linkaskflag = false;
    public List<GameObject>clientlist;

    int flag = 0, port = 0;
    // Start is called before the first frame update
    void Start()
    {
        /*maincamera = GameObject.Find("Main Camera");
        startui = GameObject.Find("startUI");
        player = game.transform.Find("Player").gameObject;
        canvas = startui.transform.Find("Canvas").gameObject;
        rightrect = canvas.transform.Find("rightrect").gameObject;
        rightrect.SetActive(false);
        netmanager = GameObject.Find("NetManager");*/
        iptext.GetComponent<TMP_Text>().text = netmanager.GetComponent<NetServer>().ip;
        porttext.GetComponent<TMP_InputField>().text = netmanager.GetComponent<NetServer>().port.ToString();
        nametext.GetComponent<TMP_InputField>().text = netmanager.GetComponent<NetManager>().localname;
        starttogamebutton.GetComponent<Button>().enabled = false;
    }

    private void Update() {
        if (linkaskflag){
            linkaskflag = false;
            linkask();
        }
    }

    // Update is called once per frame
    public void 设置click(){
        changerect("settingrect");
    }

    public void 开始click(){
        changerect("startrect");
    }

    public void startseverclick(){
        if (activerect == null || activerect.name != "startseverrect")
            netmanager.GetComponent<NetServer>().runserver();
        changerect("startseverrect");
    }

    public void bgmusicvolum(float num){
        //Debug.Log(num);
        maincamera.GetComponent<AudioSource>().volume = num;
    }
    public void setbgmusicscrool(float num){
        bgmusicsroll.GetComponent<Slider>().value = num;
    }
    public void seteffictscrool(float num){
        effictscrool.GetComponent<Slider>().value = num;
    }

    public void changerect(string name){
        Debug.Log(name);
        if (name != "" && !rightrect.activeSelf){
            rightrect.SetActive(true);
        }
        else if (name != "starttogamerect" && activerect != null && string.Compare(activerect.name,"starttogamerect")==0){
            Debug.LogWarning("stop server");
            //netmanager.GetComponent<NetServer>().stopserver();
        }
        else if (name != "settingrect" && activerect != null && string.Compare(activerect.name, "settingrect")==0){
            datamanager.GetComponent<DataManager>().datasave();
        }
        if (activerect != null)
            activerect.SetActive(false);
        activerect = rightrect.transform.Find(name).gameObject;
        activerect.SetActive(true);
    }

    public void sentlinkask(){
        string ip = serveriplable.GetComponent<TMP_InputField>().text;
        int port = int.Parse(severportlable.GetComponent<TMP_InputField>().text);
        messagepanel.SetActive(true);
        messagepanel.transform.Find("Panel/titile").GetComponent<TMP_Text>().text = "连接...";
        messagepanel.transform.Find("Panel/Button").GetComponent<Button>().onClick.AddListener(netmanager.GetComponent<NetClient>().closeclient);
        netmanager.GetComponent<NetClient>().connectserver(ip, port);
    }

    public void setlinkask(string ip, string name){
        linkaskflag = true;
        this.ip = ip;
        this.linkname = name;
    }

    public void linkask(){
        messagepanel.SetActive(true);
        messagepanel.transform.Find("messege").GetComponent<TMP_Text>().text = string.Format("{0}:{1}", linkname, ip);
        linkbutton.GetComponent<Button>().enabled = false;
    }

    public void gamereturn(){
        Debug.Log("游戏结束");
        netmanager.GetComponent<NetManager>().closelink();
        startui.SetActive(true);
        if (game != null){
            game.SetActive(false);
            Destroy(game);
        }
    }

    public void startgame(bool isserver){
        messagepanel.SetActive(false);
        messagepanel.transform.Find("Panel/Button").GetComponent<Button>().onClick.RemoveListener(netmanager.GetComponent<NetClient>().closeclient);
        startui.SetActive(false);
        netmanager.GetComponent<NetManager>().isserver = isserver;
        game = Instantiate<GameObject>(gamepre);
        player = game.transform.GetComponentInChildren<Player>();
        if(isserver){
            player.gameflag = -1;
            netmanager.GetComponent<NetServer>().startgame();
        }
        netmanager.GetComponent<NetManager>().setplayer(player);
        player.netmanager = netmanager.GetComponent<NetManager>();
        player.effictmusic = maincamera.GetComponent<EffictMusic>();
        player.startUI = this;
        game.SetActive(true);

    }

    public void linkclientclick(GameObject obj){
        netmanager.GetComponent<NetServer>().selectclient(obj.GetComponent<ItemData>().clientwork);
    }
    public void dellinkclient(Clientwork clientwork){
        foreach(Transform itemtrans in linkclientcontent.GetComponentsInChildren<Transform>()){
            if (itemtrans.gameObject.name == clientwork.clientname){
                Destroy(itemtrans.gameObject);
            }
        }
    }
    public void addlinkclient(Clientwork clientwork){
        Debug.Log("add client item");
        GameObject newitem = Instantiate<GameObject>(buttonpre);
        newitem.GetComponentInChildren<TMP_Text>().text = string.Format("{0} \n {1}", clientwork.clientname, clientwork.client.RemoteEndPoint.ToString());
        newitem.GetComponent<ItemData>().clientwork = clientwork;
        Debug.Log(string.CompareOrdinal(newitem.name, clientwork.clientname));
        newitem.GetComponent<Button>().onClick.AddListener(delegate(){ linkclientclick(newitem);});
        newitem.transform.SetParent(linkclientcontent.transform);
    }
    public void allowlinktogame(){

    }

    public void exchangegameui(){
        GameObject gameui = this.transform.Find("gameui").gameObject;
        gameui.SetActive(!gameui.activeSelf);
    }
}

