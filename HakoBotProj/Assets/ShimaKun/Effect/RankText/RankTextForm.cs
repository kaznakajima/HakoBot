using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankTextForm : MonoBehaviour
{
    //スコアを出すプレハブを呼び出すやつ
    public GameObject Rightcanvas;//右キャンバス
    public GameObject Centercanvas;//中央キャンバス
    public GameObject Leftcanvas;//左キャンバス
    public GameObject ranktext;//テキスト
    public float CreateTextTime = 1f; //テキストの呼び出し時間

    // Use this for initialization
    void Start ()
    {
        StartCoroutine("CreateText");
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    IEnumerator CreateText()
    {
        while (true)
        {
            GameObject prefab1 = (GameObject)Instantiate(ranktext);
            prefab1.transform.SetParent(Rightcanvas.transform, false);
            yield return new WaitForSeconds(CreateTextTime);
            GameObject prefab2 = (GameObject)Instantiate(ranktext);
            prefab2.transform.SetParent(Centercanvas.transform, false);
            yield return new WaitForSeconds(CreateTextTime);
            GameObject prefab3 = (GameObject)Instantiate(ranktext);
            prefab3.transform.SetParent(Leftcanvas.transform, false);
            yield return new WaitForSeconds(CreateTextTime);
        }
    }
}
