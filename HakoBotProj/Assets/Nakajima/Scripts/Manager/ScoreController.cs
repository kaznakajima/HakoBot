using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

/// <summary>
/// 得点管理クラス
/// </summary>
public class ScoreController : MonoBehaviour
{
    // 点数
    private int[] score = new int[4];

    // プレイヤーデータ
    [SerializeField]
    private List<PlayerData> playerData = new List<PlayerData>();

    // スコア用Text
    [SerializeField]
    private Text[] scoreTex;

    // 自身のオーディオ
    private AudioSource myAudio;

	// 初回処理
	void Start () {
        myAudio = GetComponent<AudioSource>();
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
        int _score = score[playerNo - 1];
        score[playerNo - 1] = nextScore;

        var disposable = new SingleAssignmentDisposable();
        // だんだんスコア加算
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(25)).Subscribe(time =>
        {
            _score++;
            scoreTex[playerNo - 1].text = _score.ToString();
            playerData[playerNo - 1].point = score[playerNo - 1];

            // 目標に達したらストップ
            if (_score == nextScore)
                disposable.Dispose();

        }).AddTo(this);
    }
}
