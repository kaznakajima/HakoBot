using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AudioList
{
    //アクセス用のキー
    public string Key;
    //リソース名
    public string ResName;
    //AudioClip
    public AudioClip Clip;

    // コンストラクタ
    public AudioList(string key, string res)
    {
        Key = key;
        ResName = "Sounds/" + res;
        //AudioClipの取得
        Clip = Resources.Load(ResName) as AudioClip;
    }
}

public class AudioController : SingletonMonobeBehaviour<AudioController>
{
    // 流すBGMのステートマシン
    public enum BGM_STATE
    {
        TITLE,
        MAIN,
        RESULT
    }
    public BGM_STATE BGMstate;

    // 流すSEのステートマシン
    public enum SE_STATE
    {
        SELECT,
        COUNTDOWN,
        START,
        END
    }
    public SE_STATE SEstate;

    // BGM、SE用のAudioSource
    [SerializeField]
    public AudioSource[] myAudio;

    // BGMのリスト
    [SerializeField]
    AudioClip[] BGM;
    // SEのリスト
    [SerializeField]
    AudioClip[] SE;

    //BGMにアクセスするためのテーブル
    Dictionary<string, AudioList> poolBgm = new Dictionary<string, AudioList>();
    //SEにアクセスするためのテーブル
    Dictionary<string, AudioList> poolSe = new Dictionary<string, AudioList>();

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadBGM("Title", "Title");
        LoadBGM("Main", "Main");
        LoadSe("Select", "Select");
        LoadSe("CountDown", "CountDown");
        LoadSe("Start", "Start");
        LoadSe("End", "End");
        LoadSe("Damage", "Damage");
        LoadSe("Release", "Release");
    }

    //サウンドのロード
    public void LoadBgm(string key, string resName)
    {
        LoadBGM(key, resName);
    }

    public void LoadSe(string key, string resName)
    {
        LoadSE(key, resName);
    }

    void LoadBGM(string key, string resName)
    {
        if (poolBgm.ContainsKey(key))
        {
            //すでに登録済みなのでいったん消す
            poolBgm.Remove(key);
        }
        poolBgm.Add(key, new AudioList(key, resName));
    }

    void LoadSE(string key, string resName)
    {
        if (poolSe.ContainsKey(key))
        {
            //すでに登録済みなのでいったん消す
            poolSe.Remove(key);
        }
        poolSe.Add(key, new AudioList(key, resName));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // BGMの変更
    public void BGMChange(string key)
    {
        // リソース作成
        var _data = poolBgm[key];
        myAudio[0].clip = _data.Clip;
        myAudio[0].Play();

        // シーンごとに音楽を設定
        switch(key)
        {
            case "Title":
                BGMstate = BGM_STATE.TITLE;
                break;
            case "Main":
                BGMstate = BGM_STATE.MAIN;
                break;
            case "Result":
                BGMstate = BGM_STATE.TITLE;
                break;
        }

        //myAudio[0].clip = BGM[(int)BGMstate];
        //myAudio[0].Play();
    }

    // SEの再生
    public void SEPlay(string key)
    {
        // リソース作成
        var _data = poolSe[key];
        myAudio[1].clip = _data.Clip;
        myAudio[1].Play();

        switch (key)
        {
            case "Select":
                SEstate = SE_STATE.SELECT;
                break;
            case "CountDown":
                SEstate = SE_STATE.COUNTDOWN;
                break;
            case "Start":
                SEstate = SE_STATE.START;
                break;
            case "End":
                SEstate = SE_STATE.END;
                break;
        }
    }

    /// <summary>
    /// 他のSEを流す
    /// </summary>
    /// <param name="otherAudio">流したい効果音をもっているAudioSource</param>
    public void OtherAuioPlay(AudioSource otherAudio, string key)
    {
        var _data = poolSe[key];
        otherAudio.clip = _data.Clip;
        otherAudio.Play();
    }
}
