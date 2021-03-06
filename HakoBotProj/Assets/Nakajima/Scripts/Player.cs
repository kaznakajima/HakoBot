﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// プレイヤークラス
/// </summary>
public class Player : MonoBehaviour, Character
{

    // インプット処理
    PlayerSystem system;

    // 自身の番号(1 → 1P, 2 → 2P, 3 → 3P, 4 → 4P)
    [SerializeField]
    private int charaNum;

    public int number
    {
        set { }
        get { return charaNum; }
    }

    // アイテムを所持しているか
    private bool hasItem;

    public bool item
    {
        set { hasItem = value; }
        get { return hasItem; }
    }

    // 自身のAnimator
    Animator myAnim;

    // 入力判定
    Vector3 inputVec;

    // アイテムを所持するための座標
    [SerializeField]
    Transform pointPos;

    // 攻撃判定
    [HideInInspector]
    public bool isAttack;

    // 移動スピード
    float runSpeed = 5.0f;

    // 自身のRig
    [HideInInspector]
    public Rigidbody myRig;

    // 自身が持っているアイテム
    [HideInInspector]
    public GameObject itemObj;

    // Use this for initialization
    void Start () {
        myAnim = GetComponent<Animator>();
        myRig = GetComponent<Rigidbody>();
        system = FindObjectOfType<PlayerSystem>();
	}
	
	// Update is called once per frame
	void Update () {
         PlayerInput();
	}

    // 入力判定
    void PlayerInput()
    {
        /* ここから移動量判定 */
        if (system.LeftStickAxis(number) != Vector2.zero)
        {
            inputVec = new Vector3(system.LeftStickAxis(number).x, 0, system.LeftStickAxis(number).y);
            Move(inputVec);
        }
        else
        {
            if (myAnim.GetInteger("PlayAnimNum") != 8)
            {
                myAnim.SetInteger("PlayAnimNum", 8);
            }
        }

        if (system.Button_A(number))
        {
            Attack();
        }
    }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    public void Move(Vector3 vec)
    {
        if (isAttack)
        {
            return;
        }

        if (myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // カメラの方向から、x-z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向の決定
        Vector3 moveForward = cameraForward * vec.z + Camera.main.transform.right * vec.x;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        myRig.velocity = moveForward * runSpeed + new Vector3(0, myRig.velocity.y, 0);

        // キャラクターの向きを進行方向に
        transform.rotation = Quaternion.LookRotation(moveForward);

    }

    // タックル
    public void Attack()
    {
        if (isAttack)
        {
            return;
        }

        myAnim.SetInteger("PlayAnimNum", 2);
        isAttack = true;

        // 1.5秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(time =>
        {
            isAttack = false;
        }).AddTo(this);
    }

    // ジャンプ
    public void Jump()
    {

    }

    // スタン
    public void Stan()
    {

    }

    /// <summary>
    /// アイテムの取得
    /// </summary>
    /// <param name="obj">アイテムのオブジェクト</param>
    public void Catch(GameObject obj)
    {
        itemObj = obj;
        if (itemObj.GetComponent<Item>().isCatch == false)
        {
            itemObj = null;
            return;
        }

        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);

        hasItem = true;
    }

    // アイテムを放棄
    public void Release()
    {
        if(itemObj == null)
        {
            return;
        }

        itemObj.GetComponent<Item>().ReleaseItem(transform.position);
        itemObj = null;
    }

    // 充電
    public void Charge()
    {

    }

    // 当たり判定
    void OnCollisionEnter(Collision col)
    {
        // アイテムだったらアイテム取得
        if (col.gameObject.name == "Item(Clone)")
        {
            Catch(col.gameObject);
        }

        // タックル中にプレイヤーに触れたとき
        if (col.gameObject.GetComponent(typeof(Character)) as Character != null && isAttack)
        {
            var character = col.gameObject.GetComponent(typeof(Character)) as Character;

            // 触れたプレイヤーがアイテムを持っていないならリターン
            if (character.item == false)
            {
                return;
            }

            Debug.Log("Release");
            character.Release();
        }
    }
}
