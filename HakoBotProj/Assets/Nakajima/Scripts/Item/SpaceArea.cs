using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceArea : MonoBehaviour
{
    // 自身のAnimator
    Animator myAnim;

    // 自身のAudioSource;
    AudioSource myAudio;

    public bool isActive;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();

        if (isActive == false) {
            Close();
            AudioController.Instance.OtherAuioPlay(myAudio, "Shutter");
        }
	}

    /// <summary>
    /// シャッターを閉じる
    /// </summary>
    public void Close()
    {
        // 機能していないフラグ
        isActive = false;

        AudioController.Instance.OtherAuioPlay(myAudio, "Shutter");

        myAnim.SetBool(gameObject.name + "_Close", true);
    }

    /// <summary>
    /// シャッターを開ける
    /// </summary>
    public void Open()
    {
        myAnim.SetBool(gameObject.name + "_Close", false);

        AudioController.Instance.OtherAuioPlay(myAudio, "Shutter");

        // 機能しているフラグ
        isActive = true;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
