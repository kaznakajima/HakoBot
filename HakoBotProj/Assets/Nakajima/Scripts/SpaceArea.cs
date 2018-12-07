using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceArea : MonoBehaviour
{
    // 自身のAnimator
    Animator myAnim;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool(gameObject.name + "_Close", true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
