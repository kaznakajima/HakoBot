using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class HPDamage : MonoBehaviour
{
    //最大HP
    [SerializeField]
    int maxHP = 100;
    //現在のHP
    [SerializeField]
    float currentHP;

    //GameObject textObj;
    Text text;
    GameObject hpSystem;

    public float interval = 0.2f;

    bool flg = false;

    [SerializeField]
    int Player_HP;


    // Use this for initialization
    void Start ()
    {
        //TextをGameObjectとして取得する
        //textObj = GameObject.Find("HPDamage");//
        //HPSystemを取得する
        hpSystem = GameObject.Find("HPCircle");
        StartCoroutine(Interval());
    }
	
	// Update is called once per frame
	void Update ()
    {
        //TextのTextコンポーネントにアクセス
        //(int)はfloatを整数で表示するためのもの
        //textObj.GetComponent<Text>().text = ((int)currentHP).ToString();
        //textObj.GetComponent<RankText>().test((int)currentHP);

        //HPSystemのスクリプトのHPDown関数に2つの数値を送る
        //hpSystem.GetComponent<HPCircle>().HPDown(currentHP, maxHP);

    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            flg = true;
            //currentHPが0以上ならTrue
            //if (0 <= currentHP || currentHP <= 100)
            //{

            //    maxHPから秒数（×10）を引いた数がcurrentHP
            //    currentHP += 10.0f;
            //}
            if(flg)
            StartCoroutine(Interval());
               
        }

        
            
            
            
            
    }

    //void Shimakun()
    //   {
    //       for (int i = 0; i < 10; i++)
    //       {
    //           interval += Time.deltaTime;
    //           if (interval >= 1.0f)
    //           {
    //               interval = 0;
    //               //  currentHPが0以上ならTrue
    //               
    //           }

    //       }flg = false;
    //   }

    private IEnumerator Interval()
    {
        for (int i = 0; i < 10; i++)
        {
            //0以上maxHP未満
            if (0 <= currentHP && currentHP <= maxHP)
            {
                //0.075ずつゲージを足している
                currentHP = 0.075f;
                hpSystem.GetComponent<HPCircle>().HPDown(currentHP, maxHP);
            }
            yield return new WaitForSeconds(interval);
        }
        flg = false;
    }
}
