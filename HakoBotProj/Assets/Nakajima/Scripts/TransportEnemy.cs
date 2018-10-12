﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;
using System.Linq;

/// <summary>
/// 荷物を運ぶことを重視した敵クラス
/// </summary>
public class TransportEnemy : EnemyBase, Character
{
    // 自身の番号(1 → 1P, 2 → 2P, 3 → 3P, 4 → 4P)
    [SerializeField]
    private int _myNumber;

    public int myNumber
    {
        set { }
        get { return _myNumber; }
    }

    // タックルエフェクト
    [SerializeField]
    GameObject _attackEffect;

    public GameObject attackEffect
    {
        set { }
        get { return _attackEffect; }
    }

    // チャージ段階
    private int _chargeLevel;

    public int chargeLevel
    {
        set { }
        get { return _chargeLevel; }
    }

    // アイテムを所持しているか
    private bool _hasItem;

    public bool hasItem
    {
        set
        {
            _hasItem = value;

            if(_hasItem == false)
            {
                ResetTarget();
            }
        }
        get { return _hasItem; }
    }

    // 自身のAnimator
    Animator myAnim;

    // Use this for initialization
    void Start () {
        myAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        myRig = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case ENEMY_STATE.PATROL:
                // 目標地点との距離が縮まったら
                float distance = Vector3.SqrMagnitude(transform.position - patrolPos);
                if (distance < 2.0f)
                {
                    // 巡回座標を初期化
                    patrolPos = Vector3.zero;
                }
                PatrolMove(patrolPos);
                SetTarget();
                break;
            case ENEMY_STATE.TARGETMOVE:
                if (targetObj != null)
                {
                    // アイテムが入手不可能ならターゲット再設定
                    if (targetObj.GetComponent<Item>() != null && targetObj.GetComponent<Item>().isCatch == false)
                    {
                        ResetTarget();
                        return;
                    }

                    Move(targetObj.transform.position);
                }
                else if(targetObj == null)
                {
                    ResetTarget();
                }
                break;
        }
        
	}

    /// <summary>
    /// ステージ上のすべてのアイテムを取得
    /// </summary>
    /// <returns>Itemクラスの配列</returns>
    private Item[] GetItems()
    {
        return FindObjectsOfType<Item>();
    }

    /// <summary>
    /// ステージ上のすべてのゴールを取得
    /// </summary>
    /// <returns>PointAreaクラスの配列</returns>
    private PointArea[] GetPointArea()
    {
        return FindObjectsOfType<PointArea>();
    }

    /// <summary>
    /// ターゲットのリセット
    /// </summary>
    public override void ResetTarget()
    {
        itemObj = null;
        targetObj = null;

        // ターゲットの設定
        SetTarget();
    }

    /// <summary>
    /// ターゲットの設定
    /// </summary>
    public override void SetTarget()
    {
        // 最短距離の初期化 (とりあえず100を入れてある)
        minDistance = 100;

        // アイテムを所持していないとき
        if (itemObj == null)
        {
            SearchTarget();
        }
        // アイテムを所持しているとき
        else
        {
            SearchPointArea();
        }
    }

    /// <summary>
    /// アイテムとの距離を取得
    /// </summary>
    public override void SearchTarget()
    {
        // ステージ上のアイテムすべてにアクセス
        for (int i = 0; i < GetItems().Length; i++)
        {
            // 最短距離のアイテムをターゲットに設定
            if (Vector3.Distance(GetItems()[i].transform.position, transform.position) < minDistance && GetItems()[i].isCatch == true)
            {
                // 最短距離の格納
                minDistance = Vector3.Distance(GetItems()[i].transform.position, transform.position);
                targetObj = GetItems()[i].gameObject;
            }
        }

        // ターゲットが設定されたらリターン
        if (minDistance != 100)
        {
            state = ENEMY_STATE.TARGETMOVE;
            return;
        }

        // ターゲットが見つからなかったら巡回
        state = ENEMY_STATE.PATROL;
    }

    /// <summary>
    /// ポイントエリアとの距離を取得
    /// </summary>
    public override void SearchPointArea()
    {
        // ステージ上のゴールすべてにアクセス
        for (int i = 0; i < GetPointArea().Length; i++)
        {
            // 最短距離のゴールをターゲットに設定
            if (Vector3.Distance(GetPointArea()[i].transform.position, transform.position) < minDistance)
            {
                // 最短距離の格納
                minDistance = Vector3.Distance(GetPointArea()[i].transform.position, transform.position);
                targetObj = GetPointArea()[i].gameObject;
            }
        }
    }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    public void Move(Vector3 vec)
    {
        if (myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        //transform.rotation = Quaternion.LookRotation(transform.forward);
        agent.SetDestination(vec);
    }

    public override void PatrolMove(Vector3 vec)
    {
        if (myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // 巡回座標が初期化されていたら
        // 再度設定
        if (patrolPos == Vector3.zero)
        {
            GetRandomPosition();
        }

        agent.SetDestination(vec);
    }

    // タックル
    public void Attack()
    {
       
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
        if (obj.GetComponent<Item>().isCatch == false)
        {
            return;
        }

        itemObj = obj;

        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);

        hasItem = true;
        SetTarget();
    }

    // アイテムを放棄
    public void Release()
    {
        if (itemObj == null)
        {
            return;
        }

        itemObj.GetComponent<Item>().ReleaseItem(transform.position);

        // ターゲットの再設定
        hasItem = false;
    }

    // 充電
    public void Charge()
    {

    }

    /// <summary>
    /// 巡回地点の取得
    /// </summary>
    /// <returns>ステージ上のランダム座標</returns>
    public override Vector3 GetRandomPosition()
    {
        // ステージのサイズ
        float stageSize = 6.0f;
        // 巡回用の座標を保存
        patrolPos = new Vector3(UnityEngine.Random.Range(-stageSize, stageSize), transform.position.y, UnityEngine.Random.Range(-stageSize, stageSize));
        return patrolPos;
    }

    // 当たり判定
    void OnCollisionEnter(Collision col)
    {
        // アイテムだったらアイテム取得
        if (col.gameObject.name == "Item(Clone)" && hasItem == false)
        {
            Catch(col.gameObject);
        }

        // タックル中にプレイヤーに触れたとき
        if (col.gameObject.GetComponent(typeof(Character)) as Character != null && isAttack)
        {
            var character = col.gameObject.GetComponent(typeof(Character)) as Character;

            // 触れたプレイヤーがアイテムを持っていないならリターン
            if (character.hasItem == false)
            {
                return;
            }

            character.Release();
        }
    }
}
