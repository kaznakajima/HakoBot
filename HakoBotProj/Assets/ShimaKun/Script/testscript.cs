﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    public int P = 0;
    //RankTextからp1を読み込むるかの確認
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //RankText.Instance.p1(1);
        //RankText.Instance.p1(2);
        MainManager.Instance.playerPoint[P].ToString();
    }
}
