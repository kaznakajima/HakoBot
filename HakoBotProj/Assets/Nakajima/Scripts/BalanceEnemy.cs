using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;

public class BalanceEnemy : EnemyBase, Character
{
    // 自身の番号(1 → 1P, 2 → 2P, 3 → 3P, 4 → 4P)
    public int _myNumber;

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
            if (_hasItem == false)
            {
                ResetTarget();
            }
        }
        get { return _hasItem; }
    }

    // 自身のAnimator
    Animator myAnim;

    // Use this for initialization
    void Start()
    {
        myAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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
                if (targetObj == null)
                {
                    SetTarget();
                }
                else
                {
                    if (targetObj.transform.parent != null && targetObj.transform.parent != this)
                        SetTarget();

                    if(targetObj.GetComponent<Item>() != null && targetObj.GetComponent<Item>().isCatch == false)
                        SetTarget();

                    Move(targetObj.transform.position);
                }
                break;
        }
    }

    /// <summary>
    /// ステージ上のすべてのプレイヤーを取得
    /// </summary>
    /// <returns>Playerクラスの配列</returns>
    private GameObject[] GetCharacter()
    {
        return GameObject.FindGameObjectsWithTag("Character");
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

        // ターゲットの設定
        SetTarget();
    }

    /// <summary>
    /// ターゲットの設定
    /// </summary>
    public override void SetTarget()
    {
        targetObj = null;

        // 最短距離の初期化 (とりあえず100を入れてある)
        minDistance = 100;

        if (itemObj == null)
        {
            SearchTarget();
        }
        else
        {
            SearchPointArea();
        }
    }

    /// <summary>
    /// 他のキャラクターとの距離を取得
    /// </summary>
    public override void SearchTarget()
    {
        // ステージ上のすべてのプレイヤーにアクセス
        for (int i = 0; i < GetCharacter().Length; i++)
        {
            // 最短距離のプレイヤーをターゲット設定
            if (Vector3.Distance(GetCharacter()[i].transform.position, transform.position) < minDistance)
            {
                var character = GetCharacter()[i].GetComponent(typeof(Character)) as Character;
                if (character.hasItem == true)
                {
                    // 最短距離の格納
                    minDistance = Vector3.Distance(GetCharacter()[i].transform.position, transform.position);
                    targetObj = GetCharacter()[i].gameObject;
                }
            }
        }

        // 誰もアイテムを所持していないなら
        // ステージ上のアイテムすべてにアクセス
        for (int i = 0; i < GetItems().Length; i++)
        {
            // 最短距離のアイテムをターゲットに設定
            if (Vector3.Distance(GetItems()[i].transform.position, transform.position) > minDistance && GetItems()[i].isCatch)
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

        // それでもターゲットが設定できないなら巡回
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
        if (isAttack)
            return;

        if (myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // ターゲットがアイテムを持っていないならターゲット変更
        if (targetObj.tag == "Character")
        {
            var character = targetObj.GetComponent(typeof(Character)) as Character;
            if (character.hasItem == false)
            {
                SetTarget();
                return;
            }
        }

        // ターゲットとの距離が近づいたら
        if (Vector3.Distance(targetObj.transform.position, transform.position) < 5.0f)
        {
            // キャラクターがターゲットでないならリターン
            if (targetObj.gameObject.tag != "Character")
            {
                return;
            }

            // パワーチャージ
            Charge();

            // 攻撃範囲に入ったら攻撃
            if (Vector3.Distance(targetObj.transform.position, transform.position) < 2.0f)
            {
                Attack();
            }

        }

        //transform.rotation = Quaternion.LookRotation(transform.forward);
        agent.SetDestination(vec);
    }

    /// <summary>
    /// 巡回移動
    /// </summary>
    /// <param name="vec">目標地点</param>
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

    /// <summary>
    /// タックル攻撃
    /// </summary>
    public void Attack()
    {
        if (isAttack)
            return;

        myAnim.SetInteger("PlayAnimNum", 2);
        isAttack = true;

        // transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * _chargeLevel, 5.0f);
        myRig.AddForce(transform.forward * _chargeLevel * 100f, ForceMode.Acceleration);

        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(time =>
        {
            _chargeLevel = 0;
            isAttack = false;
            ResetTarget();
        }).AddTo(this);
    }

    public void Jump()
    {

    }

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

        _hasItem = true;
        SetTarget();
    }

    /// <summary>
    /// アイテムの放棄
    /// </summary>
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

    /// <summary>
    /// パワーチャージ
    /// </summary>
    public void Charge()
    {
        if (_chargeLevel != 0)
            return;

        _chargeLevel = 1;

        var disposable = new SingleAssignmentDisposable();
        // 1.0秒ごとにチャージ
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(time =>
        {
            // 3段階上昇、または攻撃で終了
            if (_chargeLevel >= 2 || isAttack)
            {
                disposable.Dispose();
            }

            // チャージ段階上昇
            _chargeLevel++;
            Debug.Log("プレイヤー" + _myNumber + "パワー" + chargeLevel);

        }).AddTo(this);
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
        patrolPos = new Vector3(UnityEngine.Random.Range(-stageSize * 1.5f, stageSize * 1.5f), transform.position.y, UnityEngine.Random.Range(-stageSize, stageSize));
        return patrolPos;
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

            ResetTarget();
        }
    }
}
