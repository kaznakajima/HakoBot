using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankText : SingletonMonobeBehaviour<RankText>
{
    public int point = 1;
    public int score = 0;
    public NumSprite numSprite1; //数字sprite　1桁
    public NumSprite numSprite10; //2桁
    public NumSprite numSprite100; //3桁
    // Use this for initialization
    void Start ()
    {
        //TextController.Instance.Rank(1,1,2,1,3,1,4,1);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Rankscript.Instance.Rankscore(1, 0);
        //p1();
        //Debug.Log(Testscript.abc);
    }
    public void p1()
    { 
        if (Input.GetKey(KeyCode.A))
        {
            for (int i = 0; i < 1; i++)
            {
                score = score + point;
                if (score >= 10)//
                {
                    numSprite1.SetNumber(score % 10);
                    numSprite10.SetNumber((score / 10)%10);
                    numSprite100.SetNumber((score / 100)%10);
                }
            }           
        }
        numSprite1.SetNumber(score);          
    }
}
