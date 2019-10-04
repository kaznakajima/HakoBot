using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerBaseクラス
/// </summary>
public class PlayerBase : MonoBehaviour
{
    // 自身のレイヤー番号
    [HideInInspector]
    protected int layerNum;

    // 自身のAudio関連
    [HideInInspector]
    public AudioSource myAudio;

    // 自身のAnimator
    [HideInInspector]
    protected Animator myAnim;
    [HideInInspector]
    public Animator flashAnim;
    // 入力判定
    [HideInInspector]
    protected Vector3 inputVec;
    // アイテムを所持するための座標
    [HideInInspector]
    protected Transform pointPos;

    // 攻撃判定
    [HideInInspector]
    protected bool isAttack;

    // 移動スピード
    [HideInInspector]
    protected float runSpeed = 7.0f;

    // 自身のRig
    [HideInInspector]
    protected Rigidbody myRig;

    // 自身が持っているアイテム
    [HideInInspector]
    public GameObject itemObj;

    // エフェクト用オブジェクト
    protected GameObject stanEffect;
}
