using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handfield : MonoBehaviour
{
    int handcardmaxnum = 5;
    List<GameObject> handcards;

    public Player player;
    public List<GameObject> addingcards;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        handcards = new List<GameObject>();
        addingcards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addcard(GameObject card){
        card.transform.SetParent(this.transform);
        addingcards.Remove(card);
        handcards.Add(card);
    }

    public void handtobattle(GameObject fusedcard){
        handcards.Remove(fusedcard);
    }

    public bool hashandspase(){
        if (handcards.Count + addingcards.Count < handcardmaxnum){
            return true;
        }
        return false;
    }

    public void prephandfield(){
        int n = handcards.Count;
        if (n == 0){
            return;
        }
        handcards.Sort((x,y) => { return x.GetComponent<Card>().cardtype - y.GetComponent<Card>().cardtype;} );
        Vector3 cardsize = handcards[0].GetComponent<SpriteRenderer>().sprite.bounds.size;
        for (int i = 0; i < n; i++){
            Vector3 topositon = this.transform.position + new Vector3((i-(n-1)/2)*cardsize.x,0,0);
            handcards[i].GetComponent<Card>().cardtoposition(topositon, 0.3f);
        }
    }

    public void cardtobattle(GameObject fusedcard, GameObject target){
        fusedcard.GetComponent<Card>().position = target.GetComponent<Battle>().position;
        fusedcard.GetComponent<Card>().cardtoposition(target.transform.position, 0.3f);
        fusedcard.transform.SetParent(GameObject.Find("battlefield").transform);
        handcards.Remove(fusedcard);
        GameObject.Find("battlefield").GetComponent<Battlefield>().addcard(fusedcard, target);
        fusedcard = null;
        GameObject.Find("border").transform.position = new Vector3(-10, 0, 0);
        prephandfield();
    }

    public void enemycardtobattle(int i, int[] position){
        Card card = handcards[i].GetComponent<Card>();
        GameObject battle = GameObject.Find("battlefield").GetComponent<Battlefield>().getbattlebypos(position);
        card.cardtoposition(battle.transform.position, 0.3f);
        card.transform.SetParent(GameObject.Find("battlefield").transform);
        handcards.RemoveAt(i);
        card.transform.eulerAngles = new Vector3(0,180,180);
        card.GetComponent<Card>().position = battle.GetComponent<Battle>().position;
        prephandfield();
    }

    public int getcardindex(GameObject card){
        return handcards.IndexOf(card);
    }
}
