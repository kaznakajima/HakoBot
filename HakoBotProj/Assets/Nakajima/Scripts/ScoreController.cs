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

    // スコア加点
    public void AddScore(int playerNo)
    {
        score[playerNo - 1] += 10;
        scoreTex[playerNo - 1].text = score[playerNo - 1].ToString();
        playerData[playerNo - 1].point = score[playerNo - 1];
    }
}
