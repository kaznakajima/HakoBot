using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArea : MonoBehaviour
{
    // スコア加算
    ScoreController score;

    // ポイントエリアが機能しているか
    public bool isActive = true;

    bool isOpen = true;

    // Use this for initialization
    void Start () {
        score = FindObjectOfType<ScoreController>();
    }
	
	// Update is called once per frame
	void Update () { 
      
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.name == "Item(Clone)")
        {
            var character = col.gameObject.GetComponentInParent(typeof(Character)) as Character;
            character.hasItem = false;
            
            score.AddScore(character.myNumber);
            Destroy(col.gameObject);
        }
    }
}
