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
                if (distance < 2.0f)
                {
                    // 巡回座標を初期化
                    patrolPos = Vector3.zero;
                }
                PatrolMove(patrolPos);
                SetTarget();
                break;
            case ENEMY_STATE.TARGETMOVE:
                // ターゲットがいるなら追従
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
                // ターゲットがいないなら検索
                else if(targetObj == null)
                {
                    ResetTarget();
                }
                break;
        }
        
	}

    /// <summary>
    /// ターゲットの状態を取得
    /// </summary>
    /// <param name="otherObj">他のプレイヤー</param>
    public override void CheckTarget(GameObject otherObj)
    {
        // ターゲットリストにアクセス
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] == null || targetList[i] != targetObj || targetList.Count == 1)
                return;

            // 他のキャラクターの方がターゲットに近いならターゲット変更
            float distance = GetTargetDistance(otherObj, targetList[i]);
            if (distance < minDistance)
            {
                SetTarget();
                break;
            }
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
        maxDistance = 0;

        if (itemObj == null)
        {
            // リストを初期化
            targetList.Clear();
            SearchTarget();
        }
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
            // リストに追加
            targetList.Add(GetItems()[i].gameObject);
            // 最短距離のアイテムをターゲットに設定
            if (GetTargetDistance(GetItems()[i].gameObject, gameObject) < minDistance && GetItems()[i].isTarget == false)
            {
                // 最短距離の格納
                minDistance = GetTargetDistance(GetItems()[i].gameObject, gameObject);
                targetObj = GetItems()[i].gameObject;
            }
        }

        // ターゲットが設定されたらリターン
        if (minDistance != 100)
        {
            targetObj.GetComponent<Item>().isTarget = true;
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
        // 自身とポイントエリアの距離
        float distance;
        float[] distanceAverage = new float[4];
        float[] enemyDistacne = new float[4];

        // すべてのポイントエリアにアクセス
        for (int i = 0; i < GetPointArea().Length; i++)
        {
            distance = GetTargetDistance(gameObject, GetPointArea()[i].gameObject);
            // 最短距離のポイントエリアをターゲットとする
            if (distance < minDistance)
            {
                minDistance = distance;
                targetObj = GetPointArea()[i].gameObject;
            }

            for (int j = 0; j < GetCharacter().Length; j++)
            {
                if (GetCharacter()[i] == this)
                    return;

                enemyDistacne[j] = GetTargetDistance(GetCharacter()[j], GetPointArea()[i].gameObject);
                distanceAverage[i] += GetTargetDistance(GetCharacter()[j], GetPointArea()[i].gameObject);
                if (minDistance > enemyDistacne[j])
                {
                    targetObj = null;
                }
            }

            float average = distanceAverage[i] / 3.0f;
            if (average > maxDistance)
            {
                maxDistance = average;
                dummyTarget = GetPointArea()[i].gameObject;
            }
        }

        if (targetObj != null)
            return;

        targetObj = dummyTarget;
    }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    public void Move(Vector3 vec)
    {
        //if (targetObj.GetComponent<Item>() != null)
        //{
        //    // ターゲットの状態を確認
        //    for (int i = 0; i < GetCharacter().Length; i++)
        //    {
        //        if (GetCharacter()[i] == this)
        //            return;

        //        CheckTarget(GetCharacter()[i]);
        //    }
        //}

        if (_hasItem && myAnim.GetInteger("PlayAnimNum") != 11)
        {
            myAnim.SetInteger("PlayAnimNum", 11);
        }
        else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // 次の位置への方向を求める
        var dir = agent.nextPosition - transform.position;

        // 方向と現在の前方との角度を計算（スムーズに回転するように係数を掛ける）
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        var angle = Mathf.Acos(Vector3.Dot(transform.forward, dir.normalized)) * Mathf.Rad2Deg * smooth;

        // 回転軸を計算
        var axis = Vector3.Cross(transform.forward, dir);

        // 回転の更新
        var rot = Quaternion.AngleAxis(angle, axis);
        transform.forward = rot * transform.forward;

        //transform.rotation = Quaternion.LookRotation(angle);
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
        if (hasItem == true)
            return;

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
            return;

        myAnim.SetInteger("PlayAnimNum", 10);
        itemObj.GetComponent<Item>().ReleaseItem(transform.position);
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
        if (col.gameObject.tag == "Item")
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

    public override void OnCollisionExit(Collision col)
    {
        base.OnCollisionExit(col);
    }
}
