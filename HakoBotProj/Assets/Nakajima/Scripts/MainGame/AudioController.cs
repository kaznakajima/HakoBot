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
    // BGM、SE用のAudioSource
    public AudioSource[] myAudio;

    //BGMにアクセスするためのテーブル
    Dictionary<string, AudioList> poolBgm = new Dictionary<string, AudioList>();
    //SEにアクセスするためのテーブル
    Dictionary<string, AudioList> poolSe = new Dictionary<string, AudioList>();

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // BGM
        LoadBGM("Title", "Title");  // タイトル
        LoadBGM("Main", "Main"); // メインシーン
        LoadBGM("Result", "Result");
        // SE
        LoadSe("Select", "Select");                     // 決定
        LoadSe("CountDown", "CountDown");    // カウントダウン
        LoadSe("Start", "Start");                       // ゲームスタート
        LoadSe("End", "End");                          // ゲーム終了
        LoadSe("Damage", "Damage");             // ダメージ
        LoadSe("Release", "Release");               // 荷物リリース
        LoadSe("Entry", "Entry");                     // エントリー
        LoadSe("Warning", "Warning");            // 警告音
        LoadSe("Cancel", "Cancel");                 // キャンセル音
        LoadSe("Shutter", "Shutter");              // シャッター開閉
        LoadSe("Bomb", "Bomb");                  // 爆破音
        LoadSe("Stan", "Stan");                      // スタン
        LoadSe("Missile", "Missile");                // ミサイル
        LoadSe("AddScore", "AddScore");       // スコア加算
        LoadSe("Pause", "Pause");                 // ポーズ
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

    // BGMの変更
    public void BGMChange(string key)
    {
        // リソース作成
        var _data = poolBgm[key];
        myAudio[0].clip = _data.Clip;
        myAudio[0].Play();
    }

    // SEの再生
    public void SEPlay(string key)
    {
        if (myAudio[1].isPlaying == true && myAudio[1].clip == poolSe[key].Clip)
            return;

        // リソース作成
        var _data = poolSe[key];
        myAudio[1].clip = _data.Clip;
        myAudio[1].Play();
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
