using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    // 自身のレイヤー番号
    [HideInInspector]
    public int layerNum;

    // 自身のAudio関連
    [HideInInspector]
    public AudioSource myAudio;

    // 自身のAnimator
    [HideInInspector]
    public Animator myAnim;
    // 入力判定
    [HideInInspector]
    public Vector3 inputVec;
    // アイテムを所持するための座標
    [HideInInspector]
    public Transform pointPos;

    // 攻撃判定
    [HideInInspector]
    public bool isAttack;

    // 移動スピード
    [HideInInspector]
    public float runSpeed = 7.0f;

    // 自身のRig
    [HideInInspector]
    public Rigidbody myRig;

    // 自身が持っているアイテム
    [HideInInspector]
    public GameObject itemObj;

    // エフェクト用オブジェクト
    protected GameObject stanEffect;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
