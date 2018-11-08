using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    //RankTextからp1を読み込むるかの確認
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        RankText.Instance.p1();
    }
}
