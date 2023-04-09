using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int gameflag = 1;// 1本方回合， 2特殊卡牌时间， -1敌方回合
    public GameObject fusedcard;//聚焦的卡牌
    public string massge = null;
    public bool isreservemsg = false;
    public GameObject willwincards = null; 
    public int gamepoolnum = 0;
    public EffictMusic effictmusic = null;
    public StartUI startUI;
    public CardManager cardpoor;
    public Handfield handfield;
    public Battlefield battlefield;// 卡牌池， 手牌， 战场
    public NetManager netmanager;
    // Start is called before the first frame update
    //void Start()
    //{
        /*game = GameObject.Find("game");
        cardpoor = game.transform.Find("cardpoor").GetComponent<CardManager>();//初始化
        handfield = this.transform.Find("handfield").GetComponent<Handfield>();
        battlefield = game.transform.Find("battlefield").GetComponent<Battlefield>();
        effictmusic = GameObject.Find("Main Camera").GetComponent<EffictMusic>();
        server = GameObject.Find("NetManager").GetComponent<NetServer>();*/
    //}

    // Update is called once per frame
    void Update()
    {
        GameObject clickobj = null;//被点击对象
        if (Input.GetMouseButtonDown(0)){//射线检测
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 10);
            if (hit.collider != null){
                //Debug.Log(hit.collider.gameObject.transform.position);
                clickobj = hit.collider.gameObject;
                effictmusic.playone();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            startUI.exchangegameui();
        }

        if (gameflag == 1){//本方回合
            if (clickobj != null && clickobj.GetComponent<Card>() != null){//本方卡牌被点击
                if (clickobj.transform.parent == cardpoor.transform && handfield.hashandspase()){//卡池卡牌被点击
                    Debug.Log("true");
                    cardpoor.drawcard();//抽出一张卡
                    gameflag = -1;
                }
                else if (clickobj.transform.parent == battlefield.transform && !clickobj.GetComponent<Card>().isenemycard){//战场卡牌被点击
                    fusedcard = clickobj;
                    fusedcard.GetComponent<Card>().setborder();
                }
                else if (clickobj.transform.parent == handfield.transform){//手牌被点击
                    fusedcard = clickobj;
                    fusedcard.GetComponent<Card>().setborder();
                }
            }

            else if (clickobj != null && clickobj.GetComponent<Battle>() != null){//战场被点击
                Debug.Log(clickobj.GetComponent<Battle>().position[0]);
                if (fusedcard != null && fusedcard.transform.parent == handfield.transform && clickobj.GetComponent<Battle>().position[0] > 2){//放置卡牌至战场
                    int index = handfield.getcardindex(fusedcard);
                    handfield.cardtobattle(fusedcard, clickobj);
                    fusedcard.GetComponent<Card>().startgotobattle(clickobj);
                    int [] topos = clickobj.GetComponent<Battle>().position.Clone() as int[];
                    netmanager.sendmsg(Player.move(new int[2]{-1,index}, topos).jsontostr());
                    gameflag = -1;
                }
                else if (fusedcard != null && fusedcard.transform.parent == battlefield.transform && fusedcard.GetComponent<Card>().canmovebattle(clickobj)){//战场移动卡牌
                        Debug.Log("battle move");
                        int[] topos = clickobj.GetComponent<Battle>().position.Clone() as int[];
                        netmanager.sendmsg(Player.move(fusedcard.GetComponent<Card>().position.Clone() as int[], topos).jsontostr());
                        fusedcard.GetComponent<Card>().cardtoposition(clickobj.transform.position, 0.2f);
                        fusedcard.GetComponent<Card>().position = (int[])clickobj.GetComponent<Battle>().position.Clone();
                        if (topos[0] == 0){
                            willwincards = fusedcard;
                        }
                        else if (fusedcard.GetComponent<Card>().position[0] == 0){
                            willwincards = null;
                        }
                        gameflag = -1;
                }
            }
            else if (fusedcard != null && clickobj != null && clickobj.GetComponent<Card>().isenemycard){//敌方卡牌被点击
                if (fusedcard.transform.parent == battlefield.transform && fusedcard.GetComponent<Card>().canmovepos(clickobj.GetComponent<Card>().position) && fusedcard.GetComponent<Card>().cardtype > clickobj.GetComponent<Card>().cardtype){//吃敌方卡牌
                    netmanager.sendmsg(Player.move(fusedcard.GetComponent<Card>().position.Clone() as int[], clickobj.GetComponent<Card>().position).jsontostr());
                    fusedcard.GetComponent<Card>().cardtoposition(clickobj.transform.position, 0.2f);
                    fusedcard.GetComponent<Card>().position = clickobj.GetComponent<Card>().position.Clone() as int[];
                    Destroy(clickobj);
                    if (fusedcard.GetComponent<Card>().position[0] == 0){
                        willwincards = fusedcard;
                    }
                    else if (fusedcard.GetComponent<Card>().position[0] == 0){
                        willwincards = null;
                    }
                    gameflag = -1;
                    gameflag = -1;
                }
            }
            else if (clickobj != null && clickobj.name == "Enemyplayer" && willwincards != null){
                netmanager.sendmsg(Player.wingame().jsontostr());
            }
        }

        if (gameflag == 2){
            fusedcard.GetComponent<Card>().gotobattle();
        }

        if (this.isreservemsg == true && gameflag == -1){
            gamepoolnum ++;
            messageact();
            this.isreservemsg = false;
        }
    }

    public void sendmsg(string data){
        Debug.Log(data);
        netmanager.sendmsg(data);
    }

    public void setmsg(string msg){
        this.massge = msg;
        this.isreservemsg = true;
    }

    public void messageact(){//消息处理 同步状态
        datapack data = datapack.getdatapack(this.massge);
        Debug.Log(data.flag);
        if (data.flag == datapack.dataflag.cardpoordraw){
            Debug.Log(data.cardtype);
            cardpoor.enemycardtohand(data.cardtype);
            gameflag = 1;
        }
        else if (data.flag == datapack.dataflag.cardtobattle){
            GameObject.Find("enemyhandfield").GetComponent<Handfield>().enemycardtobattle(data.cardposition[1], data.toposition);
            gameflag = 1;
        }
        else if (data.flag == datapack.dataflag.battlecardmove){
            battlefield.battlecardmove(data.cardposition, data.toposition);
            gameflag = 1;
        }
        else if (data.flag == datapack.dataflag.wingame){
            Debug.Log("gamefail");
            gamefail();
        }

        this.isreservemsg = false;
    }

    public static datapack draw(int cardtype){
        datapack newpack = new datapack(datapack.dataflag.cardpoordraw);
        newpack.cardtype = cardtype;
        return newpack;
    }
    public static datapack move(int[] position, int[] toposition){
        datapack newpack = new datapack(datapack.dataflag.battlecardmove);
        if (position[0] == -1)
            newpack.flag = datapack.dataflag.cardtobattle;
        else
            position[0] = 5-position[0];
        toposition[0] = 5 - toposition[0];
        newpack.cardposition = position;
        newpack.toposition = toposition;
        return newpack;
    }

    public static datapack wingame(){
        return new datapack(datapack.dataflag.wingame);
    }

    void gamefail(){
        Debug.Log("game fail");
        startUI.gamereturn();
    }

}

public class datapack
{
    public enum dataflag{
        viodflag,
        cardpoordraw,
        cardtobattle,
        battlecardmove,
        wingame,
    }
    public dataflag flag = dataflag.viodflag;
    public int cardtype = 0;
    public int[] cardposition = new int[]{0,0};
    public int[] toposition = new int[]{0,0};

    public datapack(){
    }
    public datapack(dataflag flag){
        this.flag = flag;
    }

    public string jsontostr(){
        return JsonUtility.ToJson(this);
    }

    public static datapack getdatapack(string msg){
        return JsonUtility.FromJson<datapack>(msg);
    }
}