﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オーディオを格納するクラス
/// </summary>
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

/// <summary>
/// オーディオを管理するクラス
/// </summary>
public class AudioController : SingletonMonobeBehaviour<AudioController>
{
    // BGM、SE用のAudioSource
    public AudioSource[] myAudio;

    //BGMにアクセスするためのテーブル
    Dictionary<string, AudioList> poolBgm = new Dictionary<string, AudioList>();
    //SEにアクセスするためのテーブル
    Dictionary<string, AudioList> poolSe = new Dictionary<string, AudioList>();

    // 消音判定
    [HideInInspector]
    public bool volumeZero;

    /// <summary>
    /// 事前準備
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    // 初回処理
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
        LoadSe("Sparke", "Sparke");               // 電撃
        LoadSe("Missile", "Missile");                // ミサイル
        LoadSe("AddScore", "AddScore");       // スコア加算
        LoadSe("Pause", "Pause");                 // ポーズ
    }

    /// <summary>
    /// BGMロード(外部アクセス)
    /// </summary>
    /// <param name="key">keyネーム</param>
    /// <param name="resName">ファイル名</param>
    public void LoadBgm(string key, string resName)
    {
        LoadBGM(key, resName);
    }

    /// <summary>
    /// SEロード(外部アクセス)
    /// </summary>
    /// <param name="key">keyネーム</param>
    /// <param name="resName">ファイル名</param>
    public void LoadSe(string key, string resName)
    {
        LoadSE(key, resName);
    }

    /// <summary>
    /// BGMロード
    /// </summary>
    /// <param name="key">keyネーム</param>
    /// <param name="resName">ファイル名</param>
    void LoadBGM(string key, string resName)
    {
        // 登録済みの場合削除
        if (poolBgm.ContainsKey(key)) {
            poolBgm.Remove(key);
        }
        poolBgm.Add(key, new AudioList(key, resName));
    }

    /// <summary>
    /// SEロード
    /// </summary>
    /// <param name="key">keyネーム</param>
    /// <param name="resName">ファイル名</param>
    void LoadSE(string key, string resName)
    {
        // 登録済みの場合削除
        if (poolSe.ContainsKey(key)) {
            poolSe.Remove(key);
        }
        poolSe.Add(key, new AudioList(key, resName));
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    /// <param name="key">keyネーム</param>
    public void BGMChange(string key)
    {
        // リソース作成
        var _data = poolBgm[key];
        myAudio[0].clip = _data.Clip;
        myAudio[0].Play();
    }

    /// <summary>
    /// SEの再生
    /// </summary>
    /// <param name="key">keyネーム</param>
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
        if (volumeZero) otherAudio.volume = 0.0f;

        // リソース作成
        var _data = poolSe[key];
        otherAudio.clip = _data.Clip;
        otherAudio.Play();
    }
}
