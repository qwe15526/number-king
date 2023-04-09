using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffictMusic : MonoBehaviour
{
    AudioSource musicplayer;
    AudioClip audios;
    public float volume = 0.5f;
    public StartUI startUI;
    // Start is called before the first frame update
    void Start()
    {
        audios = Resources.Load<AudioClip>("clickaudio.mp3");
        musicplayer = this.gameObject.GetComponent<AudioSource>();
    }

    public void playone(){
        musicplayer.PlayOneShot(audios);
    }

    public void setmusicvolum(float volum){
        this.GetComponent<AudioSource>().volume = volum;
        startUI.setbgmusicscrool(volum);
    }
    
    public void setvolum(float volum){
        this.volume = volum;
        startUI.seteffictscrool(this.volume);
    }
}
