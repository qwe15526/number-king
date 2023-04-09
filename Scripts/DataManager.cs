using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public Player player;
    public NetManager netManager;
    public StartUI startui;
    public EffictMusic effictMusic;
    public GameData data = new GameData();
    public string path = Application.streamingAssetsPath+ "\\set.json";
    public void Start()
    {
        path = Application.streamingAssetsPath+ "\\set.json";
        using(StreamReader file = File.OpenText(path)){
            data = JsonUtility.FromJson<GameData>(file.ReadToEnd());
            netManager.localname = data.localname;
            netManager.GetComponent<NetServer>().port = data.port;
            effictMusic.setvolum(data.effictvolume);
            effictMusic.setmusicvolum(data.volume);
        }
    }


    public void datasave(){
        try{
            data.localname = netManager.localname;
            data.port = netManager.GetComponent<NetServer>().port;
            data.effictvolume = effictMusic.volume;
            data.volume = effictMusic.GetComponent<AudioSource>().volume;
            using (StreamWriter file = new StreamWriter(path)){
                Debug.Log(JsonUtility.ToJson(data));
                file.Write(JsonUtility.ToJson(data));
            }
        }
        catch(System.Exception ex){
            Debug.Log(ex.Message);
        }


    }

    private void OnApplicationQuit() {
        datasave();
    }
}

public struct GameData
{
    public string localname;
    public int port;
    public float volume;
    public float effictvolume;
}