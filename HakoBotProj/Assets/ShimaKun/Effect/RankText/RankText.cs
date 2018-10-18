using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankText : MonoBehaviour
{
    public TryScript tryScript;
    // Use this for initialization
    void Start ()
    {
        int RankText;
        //         スクリプト名、変数名
        RankText = tryScript.text;
        this.GetComponent<Text>().text = "スコア"+ RankText.ToString()+"点";
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }
}
