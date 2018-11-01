using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using System;

public class AttackEnemy : EnemyBase, Character
{
    // エフェクト再生
    EffekseerEmitter emitter;

    // 自身の番号(1 → 1P, 2 → 2P, 3 → 3P, 4 → 4P)
    public int _myNumber;

    public int myNumber
    {
        set { }
        get { return _myNumber; }
    }

    // 自身のエネルギー残量
    private int _myEnergy = 100;

    public int myEnergy
    {
        set { }
        get { return _myEnergy; }
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
        pointPos = GetComponentInChildren<EffekseerEmitter>().gameObject.transform;
        myAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        myRig = GetComponent<Rigidbody>();
        emitter = GetComponentInChildren<EffekseerEmitter>();
    }

    // Update is called once per frame
    void Update () {
        switch (state)
        {
            case ENEMY_STATE.PATROL:
                // 目標地点との距離が縮まったら
                float distance = Vector3.SqrMagnitude(transform.position - patrolPos);
                if(distance < 2.0f)
                {
                    // 巡回座標を初期化
                    patrolPos = Vector3.zero;
                }
                PatrolMove(patrolPos);
                SetTarget();
                break;
            case ENEMY_STATE.TARGETMOVE:
                // ターゲットがいないならターゲット検索
                if (targetObj == null)
                {
                    ResetTarget();
                }
                // ターゲットがいるなら追従
                else if(targetObj != null)
                {
                    if (targetObj.transform.parent != null && targetObj.transform.parent != this)
                        SetTarget();

                    Move(targetObj.transform.position);
                }
                break;
        }
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

        if(itemObj == null)
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
            if (GetTargetDistance(GetCharacter()[i], gameObject) < minDistance)
            {
                var character = GetCharacter()[i].GetComponent(typeof(Character)) as Character;
                if (character.hasItem == true)
                {
                    // 最短距離の格納
                    minDistance = GetTargetDistance(GetCharacter()[i], gameObject);
                    targetObj = GetCharacter()[i];
                }
            }
        }

        // ターゲットが設定されたらリターン
        if (minDistance != 100)
        {
            state = ENEMY_STATE.TARGETMOVE;
            return;
        }

        // 誰もアイテムを所持していないなら
        // ステージ上のアイテムすべてにアクセス
        for (int i = 0; i < GetItems().Length; i++)
        {
            // 最短距離のアイテムをターゲットに設定
            if (GetTargetDistance(GetItems()[i].gameObject, gameObject) < minDistance && GetItems()[i].isCatch == true)
            {
                // 最短距離の格納
                minDistance = GetTargetDistance(GetItems()[i].gameObject, gameObject);
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
            if (GetTargetDistance(GetPointArea()[i].gameObject, gameObject) < minDistance)
            {
                // 最短距離の格納
                minDistance = GetTargetDistance(GetPointArea()[i].gameObject, gameObject);
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

        if(_hasItem && myAnim.GetInteger("PlayAnimNum") != 11)
        {
            myAnim.SetInteger("PlayAnimNum", 11);
        }
        else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // ターゲットがアイテムを持っていないならターゲット変更
        if (targetObj.tag == "Character")
        {
            var character = targetObj.GetComponent(typeof(Character)) as Character;
            if (character.hasItem == false)
            {
                ResetTarget();
                return;
            }
        }

        agent.SetDestination(vec);

        // ターゲットとの距離が近づいたら
        if (GetTargetDistance(targetObj, gameObject) < 5.0f)
        {
            // キャラクターがターゲットでないならリターン
            if (targetObj.gameObject.tag != "Character")
                return;

            // パワーチャージ
            Charge();

            // 攻撃範囲に入ったら攻撃
            if (GetTargetDistance(targetObj, gameObject) < 2.0f)
            {
                Attack();
            }

        }
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
        if(patrolPos == Vector3.zero)
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

        // エフェクト再生
        emitter.Play();

        myAnim.SetInteger("PlayAnimNum", 1);
        isAttack = true;

        // transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * _chargeLevel, 5.0f);
        myRig.AddForce(transform.forward * _chargeLevel / 1.5f * 200f, ForceMode.Acceleration);

        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(time =>
        {
            // チャージ段階を初期化
            _chargeLevel = 0;
            isAttack = false;
            SetTarget();
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
        if (obj.GetComponent<Item>().isCatch == false)
            return;

        myAnim.SetInteger("PlayAnimNum", 12);

        itemObj = obj;

        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);

        hasItem = true;
        SetTarget();
    }

    /// <summary>
    /// アイテムの放棄
    /// </summary>
    public void Release()
    {
        if (itemObj == null)
            return;

        myAnim.SetInteger("PlayAnimNum", 10);

        itemObj.GetComponent<Item>().ReleaseItem(transform.position);

        // ターゲットの再設定
        hasItem = false;
    }

    /// <summary>
    /// パワーチャージ
    /// </summary>
    public void Charge()
    {
        if (_chargeLevel != 0)
            return;

        _chargeLevel = 1;
        emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();

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
            emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();
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
                return;

            character.Release();
        }
    }

    public override void OnCollisionExit(Collision col)
    {
        base.OnCollisionExit(col);
    }
}
