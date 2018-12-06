using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//保存するデータ
class Data
{
    //アクセス用のキー
    public string Key;
    //リソース名
    public string ResName;
    //AudioClip
    public AudioClip Clip;
    //コンストラクタ
    public Data(string key,string res)
    {
        Key = key;
        ResName = "Sounds/" + res;
        //AudioClipの取得
        Clip = Resources.Load(ResName) as AudioClip;

    }
}
public class Sound : MonoBehaviour {
    //SEチャンネル数
    const int SE_CHANNEL = 3;
    //サウンドの種類
    enum SEType
    {
        BGM,//BGM
        SE,//SE
    }
    //シングルトン
    static Sound singleton = null;
    //インスタンス取得
    public static Sound Instance()
    {
        return singleton ?? (singleton = new Sound());
    }
    //サウンド再生のためのゲームオブジェクト
    GameObject _object = null;
    //サウンドリソース
    AudioSource bgm = null;//BGM
    AudioSource SEDefault;//SE(デフォルト)
    AudioSource[] SEArray;//SE(チャンネル)
    //BGMにアクセスするためのテーブル
    Dictionary<string, Data> poolBgm = new Dictionary<string, Data>();
    //SEにアクセスするためのテーブル
    Dictionary<string, Data> poolSe = new Dictionary<string, Data>();
    //コンストラクタ
    public Sound()
    {
        //  チャンネルの確保
        SEArray = new AudioSource[SE_CHANNEL];
    }
    //サウンドのロード
    public static void LoadBgm(string key,string resName)
    {
        Instance().LoadBGM(key, resName);
    }
    public static void LoadSe(string key,string resName)
    {
        Instance().LoadSE(key, resName);
    }
    void LoadBGM(string key,string resName)
    {
        if (poolBgm.ContainsKey(key))
        {
            //すでに登録済みなのでいったん消す
            poolBgm.Remove(key);
        }
        poolBgm.Add(key, new Data(key, resName));
    }
    void LoadSE(string key,string resName)
    {
        if (poolSe.ContainsKey(key))
        {
            //すでに登録済みなのでいったん消す
            poolSe.Remove(key);
        }
        poolSe.Add(key, new Data(key, resName));
    }
    //BGMの再生
    public static bool PlayBGM(string key)
    {
        return Instance().PlayBgm(key);
    }
    bool PlayBgm(string key)
    {
        if (poolBgm.ContainsKey(key) == false)
        {
            //対応するキーがない
            return false;
        }
        //いったん止める
        _StopBgm();
        //リソースの取得
        var data = poolBgm[key];
        //再生
        var source = GetAudioSource(SEType.BGM);
        source.loop = true;
        source.clip = data.Clip;
        source.Play();
        

        return true;
    }
    //AudioSoureを取得する
    AudioSource GetAudioSource(SEType type,int channel = 1)
    {
        if (_object == null)
        {
            //ゲームオブジェクトがなければ作る
            _object = new GameObject("Sound");
            //廃棄しないようにする
            GameObject.DontDestroyOnLoad(_object);
            //AudioSourceを作成
            bgm = _object.AddComponent<AudioSource>();
            SEDefault = _object.AddComponent<AudioSource>();
            for (int i = 0; i < SE_CHANNEL; i++)
            {
                SEArray[i] = _object.AddComponent<AudioSource>();
            }

        }
        if (type == SEType.BGM)
        {
            //BGM
            return bgm;
        }
        else
        {
            //SE
            if (0 <= channel && channel < SE_CHANNEL)
            {
                //チャンネルの指定
                return SEArray[channel];
            }
            else
            {
                //デフォルト
                return SEDefault;
            }
        }
    }
    //BGMの停止
    public static bool StopBgm()
    {
        return Instance()._StopBgm();
    }
    bool _StopBgm()
    {
        GetAudioSource(SEType.BGM).Stop();
        return true;
    }
    //SEの再生
    public static bool PlaySe(string key,int channel = 1)
    {
        return Instance().PlaySE(key, channel);
    }
    bool PlaySE(string key,int channel = 1)
    {
        if (poolSe.ContainsKey(key) == false)
        {
            //対応するキーがない
            return false;
        }
        //リソースの取得
        var _data = poolSe[key];
        if (0 <= channel && channel < SE_CHANNEL)
        {
            //チャンネルの指定
            var source = GetAudioSource(SEType.SE, channel);
            source.clip = _data.Clip;
            source.Play();
        }
        else
        {
            //デフォルトで再生
            var source = GetAudioSource(SEType.SE);
            source.PlayOneShot(_data.Clip);
        }
        return true;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
