using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumSprite : MonoBehaviour
{
    //数字の表示
    public Sprite[] number = new Sprite[10];
    

    public void SetNumber(int num)
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = number[num];
        Debug.Log(number[num]);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
