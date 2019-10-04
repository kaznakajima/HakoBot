using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミサイル発射口クラス
/// </summary>
public class SpaceArea : MonoBehaviour
{
    // 自身のAnimator
    private Animator myAnim;

    // 自身のAudioSource;
    private AudioSource myAudio;

    // アクティブかどうか
    public bool isActive;

	// 初回処理
	void Start () {
        myAnim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();

        // 初回は閉じる
        Close();
        AudioController.Instance.OtherAuioPlay(myAudio, "Shutter");
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
}
