using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArea : MonoBehaviour
{
    // スコア加算
    ScoreController score;

    // ポイントエリアが機能しているか
    public bool isActive = true;

    // アイテムを運ぶ方向
    [SerializeField]
   Vector3 dir;

    // ターゲットポイント
    public GameObject targetObj;

    // Use this for initialization
    void Start () {
        score = FindObjectOfType<ScoreController>();
    }
	
	// Update is called once per frame
	void Update () { 
      
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
