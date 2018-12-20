using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    // 点数
    int[] score = new int[4];

    [SerializeField]
    private List<PlayerData> playerData = new List<PlayerData>();

    [SerializeField]
    private Text[] scoreTex;

	// Use this for initialization
	void Start () {
		
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
        score[playerNo - 1] += point;
        scoreTex[playerNo - 1].text = score[playerNo - 1].ToString();
        playerData[playerNo - 1].point = score[playerNo - 1];
    }
}
