using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // BGMの変更
    public void BGMChange(string scene)
    {
        // シーンごとに音楽を設定
        switch(scene)
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

        myAudio[0].clip = BGM[(int)BGMstate];
        myAudio[0].Play();
    }

    // SEの再生
    public void SEPlay(string audio)
    {
        switch(audio)
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

        myAudio[1].clip = SE[(int)SEstate];
        myAudio[1].Play();
    }

    /// <summary>
    /// 他のSEを流す
    /// </summary>
    /// <param name="otherAudio">流したい効果音をもっているAudioSource</param>
    public void OtherAuioPlay(AudioSource otherAudio, AudioClip playClip)
    {
        otherAudio.clip = playClip;
        otherAudio.Play();
    }
}
