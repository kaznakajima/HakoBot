using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceArea : MonoBehaviour
{
    // 自身のAnimator
    Animator myAnim;

    // 自身のAudioSource;
    AudioSource myAudio;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        //myAnim.SetBool(gameObject.name + "_Close", true);
        //AudioController.Instance.OtherAuioPlay(myAudio, "Shutter");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
