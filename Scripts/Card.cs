using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardtype;
    int flag = 0;
    public Vector2 poortohand;
    Vector3 poortohandeuler = new Vector3(0,180,0);
    Vector3 toposition;
    Vector3 topositionfact;
    Vector3 toeuler;
    public bool isenemycard = false;
    float movetime;
    float rotatetime;
    float movedttime = 0f, rotatedttime = 0f;
    Vector3 torotate;
    public int[] position = new int[2]{0,0};

    static string[] inttypes = new string[11]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "x"};

    public Handfield handfield;
    public Player player;
    // Start is called before the first frame update
    public void Start()
    {
        handfield = GameObject.Find("handfield").GetComponent<Handfield>();
        player = GameObject.Find("Player").GetComponent<Player>();
        poortohand = handfield.transform.position - GameObject.Find("cardpoor").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (flag == 1){
            cardtohand();
        }
        if (flag == 2 ){
            cardmovebytime();
        }
        
    }

    public void settype(int type){
        this.cardtype = type;
    }

    public void cardtohand(){
        if (flag != 1){
            poortohand = handfield.transform.position - this.transform.position;
            Debug.Log(this.handfield);
            this.handfield.addingcards.Add(this.gameObject);
            //this.transform.SetParent(GameObject.Find("csloador").transform);
            flag = 1;
        }
        movedttime += Time.deltaTime;
        this.transform.Translate(poortohand*Time.deltaTime, Space.World);
        this.transform.Rotate(poortohandeuler, 180*Time.deltaTime, Space.World);
        if (movedttime > 0.998f){
            this.transform.position = this.handfield.transform.position;
            this.transform.eulerAngles = poortohandeuler;
            flag = 0;
            movedttime = 0f;
            this.handfield.addcard(this.gameObject);
            this.handfield.prephandfield();
        }
    }

    public void cardtoeuler(Vector3 euler, float time){
        toeuler = euler;
        rotatetime = time;
        flag = 2;
    }

    public void cardtoposition(Vector3 position, float time){
        position[2] = this.transform.position[2];
        toposition = position;
        topositionfact = position - this.transform.position;
        movetime = time;
        flag = 2;
    }

    public void cardrotatebytime(){
        rotatedttime += Time.deltaTime;
        this.transform.Rotate(toeuler, rotatedttime/rotatetime);
        if (rotatedttime > rotatetime-0.002){
            this.transform.eulerAngles = toeuler;
            flag = 0;
            rotatetime = 0;
            rotatedttime = 0.0f;
        }
    }

    void cardmovebytime(){
        movedttime += Time.deltaTime;
        this.transform.Translate(topositionfact*Time.deltaTime*1/movetime, Space.World);
        if (movedttime > movetime-0.002){
            this.transform.position = toposition;
            flag = 0;
            movetime = 0;
            movedttime = 0f;
        }
    }

    public void setborder(){
        GameObject border = GameObject.Find("border");
        border.transform.SetParent(this.transform);
        border.transform.localPosition = Vector3.zero;
    }

    public bool canmovebattle(GameObject battle){
        int[] pos = battle.GetComponent<Battle>().position;
        pos = new int[2]{this.position[0]-pos[0], this.position[1]-pos[1]};
        if (-1 <= pos[0] && pos[0] <= 1 && -1 <= pos[1] && pos[1] <= 1 && Mathf.Abs(pos[0]) + Mathf.Abs(pos[1]) != 2){
            return true;
        }
        return false;
    }

    public bool canmovepos(int[] pos){
        pos = new int[2]{this.position[0]-pos[0], this.position[1]-pos[1]};
        if (-1 <= pos[0] && pos[0] <= 1 && -1 <= pos[1] && pos[1] <= 1 && Mathf.Abs(pos[0]) + Mathf.Abs(pos[1]) != 2){
            return true;
        }
        return false;
    }

    public void startgotobattle(GameObject battle){
        if (cardtype == 11){
            foreach (Card card in GameObject.Find("battlefield").GetComponent<Battlefield>().transform.GetComponentsInChildren<Card>()){
                if (this.canmovepos(card.position)){
                    if (player.gameflag == 2)
                        player.gameflag = 2;
                }
            }
            if (player.gameflag != 2){
                this.cardtype = 5;
            }
        }
    }

    public void enemycard(){
        isenemycard = true;
        this.poortohandeuler = new Vector3(0,0,180);
        this.GetComponent<Card>().handfield = GameObject.Find("enemyhandfield").GetComponent<Handfield>();
    }

    public void gotobattle(){

    }

}
