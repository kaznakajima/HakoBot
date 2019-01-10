using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ScoreController : MonoBehaviour
{
    // 点数
    int[] score = new int[4];

    [SerializeField]
    private List<PlayerData> playerData = new List<PlayerData>();

    [SerializeField]
    private Text[] scoreTex;

    AudioSource myAudio;

	// Use this for initialization
	void Start () {
        myAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// スコア加点
    /// </summary>
    /// <param name="playerNo">プレイヤー番号</param>
    /// <param name="point">加算する点数</param>
    public void AddScore(int playerNo, int point)
    {
        AudioController.Instance.OtherAuioPlay(myAudio, "AddScore");

        // 目標の得点を格納
        int nextScore = score[playerNo - 1] + point;

        var disposable = new SingleAssignmentDisposable();
        // だんだんスコア加算
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(50)).Subscribe(time =>
        {
            score[playerNo - 1]++;
            scoreTex[playerNo - 1].text = score[playerNo - 1].ToString();
            playerData[playerNo - 1].point = score[playerNo - 1];

            // 目標に達したらストップ
            if (score[playerNo - 1] == nextScore)
                disposable.Dispose();

        }).AddTo(this);
    }
}
