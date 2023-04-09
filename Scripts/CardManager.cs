using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    int cardnum = 10;
    public GameObject cardpre, enemycardpre;
    public Sprite[] cards = new Sprite[11];
    public List<GameObject> cardpoor;
    GameObject drawedcard;

    Player player;
    GameObject handfield;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        handfield = GameObject.Find("handfield");
        /*for (int i = 0; i < 11; i++){
            string id = i.ToString();
            if (i == 10){
                id = "x";
            }
            cards[i] = Unity.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Image/card/card" + id + ".png");
        }*/
        cardpoor = new List<GameObject>();
        for (int i = 0; i < 10; i++){
            for (int n = 0; n < cardnum; n++){
                GameObject newcard = Instantiate(cardpre);
                newcard.transform.SetParent(this.transform);
                newcard.transform.Find("card").GetComponent<SpriteRenderer>().sprite = cards[i];
                newcard.transform.GetComponent<Card>().settype(i);
                newcard.transform.localPosition = new Vector3(-(i+n)*0.01f,(i+n)*0.01f,0);
                cardpoor.Add(newcard);
                if (i == 10 && n == 4){
                    break;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Card getrandomcard(){
        int num = Random.Range(0, cardpoor.Count);
        return cardpoor[num].GetComponent<Card>();
    }
    public GameObject drawcard(){
        int num = Random.Range(0, cardpoor.Count);
        exchange(num);
        drawedcard = cardpoor[0];
        cardpoor.RemoveAt(0);
        drawedcard.GetComponent<Card>().cardtohand();
        player.sendmsg(Player.draw(drawedcard.GetComponent<Card>().cardtype).jsontostr());
        return drawedcard;
    }

    public void enemycardtohand(int cardtype){
        GameObject seleted = null;
        for(int i = 0; i < cardpoor.Count; i++){
            if (cardpoor[i].GetComponent<Card>().cardtype == cardtype){
                seleted = cardpoor[i];
                exchange(i);
                break;
            }
        }
        cardpoor.RemoveAt(0);
        seleted.GetComponent<Card>().enemycard();
        seleted.GetComponent<Card>().cardtohand();
    }

    void exchange(int i){
        if (cardpoor.Count > 1){
            GameObject card = cardpoor[0];
            cardpoor[0] = cardpoor[i];
            cardpoor[i] = card;
        }
    }
}
