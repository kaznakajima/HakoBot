using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 得点エリアクラス
/// </summary>
public class PointArea : MonoBehaviour
{
    // スコア加算
    private ScoreController score;

    // ターゲットにされているか
    public bool isTarget = false;
    // ポイントエリアが機能しているか
    public bool isActive = true;
    // ランプ用のRenderer
    [SerializeField]
    private MeshRenderer[] lampRenderer;

    // 自身のAnimator
    private Animator myAnim;

    // アイテムを運ぶ方向
    [SerializeField]
    private Vector3 dir;

    // ターゲットポイント
    public GameObject targetObj;

    // 自身のAudioSource
    private AudioSource myAudio;

    // 初回処理
    void Start () {
        myAnim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        score = FindObjectOfType<ScoreController>();
    }

    /// <summary>
    /// シャッターを閉じる
    /// </summary>
    public void Close()
    {
        // 機能していないフラグ
        isActive = false;

        AudioController.Instance.OtherAuioPlay(myAudio, "Warning");

        LampController.Instance.LampChange(lampRenderer, LampController.LAMP_LIGHT.RED);

        myAnim.SetBool(gameObject.name + "_Close", true);
    }

    /// <summary>
    /// シャッターを開ける
    /// </summary>
    public void Open()
    {
        myAnim.SetBool(gameObject.name + "_Close", false);
        
        AudioController.Instance.OtherAuioPlay(myAudio, "Shutter");

        LampController.Instance.LampChange(lampRenderer, LampController.LAMP_LIGHT.BLUE);

        // 機能しているフラグ
        isActive = true;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="col">離れたCollision</param>
    void OnTriggerEnter(Collider col)
    {
        // アイテムだった場合
        if(col.gameObject.tag == "Item")
        {
            var character = col.gameObject.GetComponentInParent<Character>();
            character.ItemCarry();
            
            score.AddScore(character.myNumber, col.gameObject.GetComponent<Item>().point);
            col.gameObject.GetComponent<Item>().ItemCarry(gameObject, dir);
        }
    }
}
