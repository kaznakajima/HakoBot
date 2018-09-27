using System.Collections;
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

            // アイテムを持っていないなら
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
		if(targetObj != null)
        {
            // アイテムが入手不可能ならターゲット再設定
            if(targetObj.GetComponent<Item>() != null && targetObj.GetComponent<Item>().isCatch == false)
            {
                ResetTarget();
                return;
            }

            Move(targetObj.transform.position);
        }
        else
        {
            ResetTarget();
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
        _hasItem = false;
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
            // ステージ上のアイテムすべてにアクセス
            for (int i = 0; i < GetItems().Length; i++)
            {
                // 最短距離のアイテムをターゲットに設定
                if (Vector3.Distance(GetItems()[i].transform.position, transform.position) < minDistance && GetItems()[i].isCatch)
                {
                    // 最短距離の格納
                    minDistance = Vector3.Distance(GetItems()[i].transform.position, transform.position);
                    targetObj = GetItems()[i].gameObject;
                }
            }
        }
        // アイテムを所持しているとき
        else
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

        itemObj = obj;
        if (itemObj.GetComponent<Item>().isCatch == false)
        {
            itemObj = null;
            return;
        }

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
        ResetTarget();
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
            if (character.hasItem == false)
            {
                return;
            }

            character.Release();
        }
    }
}
