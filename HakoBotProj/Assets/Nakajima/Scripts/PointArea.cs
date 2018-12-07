using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArea : MonoBehaviour
{
    // スコア加算
    ScoreController score;

    // ポイントエリアが機能しているか
    public bool isActive = true;

    // 自身のAnimator
    Animator myAnim;

    // アイテムを運ぶ方向
    [SerializeField]
   Vector3 dir;

    // ターゲットポイント
    public GameObject targetObj;

    // Use this for initialization
    void Start () {
        myAnim = GetComponent<Animator>();
        score = FindObjectOfType<ScoreController>();
    }
	
	// Update is called once per frame
	void Update () { 
      
	}

    /// <summary>
    /// シャッターを閉じる
    /// </summary>
    public void Close()
    {
        // 機能していないフラグ
        isActive = false;

        myAnim.SetBool(gameObject.name + "_Close", true);
    }

    /// <summary>
    /// シャッターを開ける
    /// </summary>
    public void Open()
    {
        myAnim.SetBool(gameObject.name + "_Close", false);

        // 機能しているフラグ
        isActive = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Item")
        {
            var character = col.gameObject.GetComponentInParent(typeof(Character)) as Character;
            character.hasItem = false;
            
            score.AddScore(character.myNumber);
            col.gameObject.GetComponent<Item>().ItemCarry(gameObject, dir);
        }
    }
}
