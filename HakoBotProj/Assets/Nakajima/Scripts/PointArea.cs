﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArea : MonoBehaviour
{

    ScoreController score;

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
            character.item = false;
            score.AddScore(character.number);
            Destroy(col.gameObject);
        }
    }
}
