using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public GameObject battlepre;
    public GameObject[,] battelfield;
    int[] size = new int[2]{6,3};

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        battelfield = new GameObject[size[0],size[1]];
        float inter = 0.1f;
        Vector3 bgsize = battlepre.GetComponent<SpriteRenderer>().sprite.bounds.size;
        for (int l = 0; l < 6; l++){
            for (int n = 0; n < 3; n++){
                GameObject newbattle = Instantiate<GameObject>(battlepre);
                newbattle.transform.SetParent(this.transform);
                newbattle.transform.localPosition = new Vector3((n-1)*(bgsize.x+inter), -(l-2)*(bgsize.y+inter)+bgsize.y/2+inter/2, 3);
                newbattle.GetComponent<Battle>().position = new int[2]{l,n};
                battelfield[l,n] = newbattle;
            }
        }
    }


    public void movecard(){

    }

    public void addcard(GameObject card, GameObject battle){
        //card.GetComponent<Card>().position = battle.GetComponent<Battle>().position;
    }

    public int[] getbattlepos(GameObject battle){
        for ( int l = 0; l < 6; l++){
            for (int n = 0; n < 3; n++){
                if (battle == battelfield[l,n]){
                    return new int[2]{l,n};
                }
            }
        }
        return null;
    }

    public GameObject getbattlebypos(int[] pos){
        return battelfield[pos[0], pos[1]];
    }

    public GameObject getcardbypos(int[] pos){
        foreach(Card card in this.transform.GetComponentsInChildren<Card>()){
            Debug.Log(card.position);
            if (card.position[0] == pos[0] && card.position[1] == pos[1]){
                return card.gameObject;
            }
        }
        return null;
    }

    public void battlecardmove(int[] pos, int[] topos){
        /*GameObject battle = GameObject.Find("battlefield").GetComponent<Battlefield>().getbattlebypos(pos);
        if (canmovebattle(battle)){
            
        }*/
        GameObject card = GameObject.Find("battlefield").GetComponent<Battlefield>().getcardbypos(pos);
        GameObject battle = GameObject.Find("battlefield").GetComponent<Battlefield>().getbattlebypos(topos);
        GameObject tocard = this.getcardbypos(topos);
        if (tocard){
            Destroy(tocard);
        }
        card.GetComponent<Card>().cardtoposition(battle.transform.position, 0.3f);
        card.GetComponent<Card>().position = battle.GetComponent<Battle>().position;
    }
    public void cardmove(GameObject card, GameObject battle){
        
    }
}
