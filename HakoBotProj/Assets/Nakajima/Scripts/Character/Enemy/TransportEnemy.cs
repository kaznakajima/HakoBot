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
    // EffekseerEmitter emitter;

    // 自身の番号(1 → 1P, 2 → 2P, 3 → 3P, 4 → 4P)
    public int _myNumber;

    public int myNumber
    {
        set { }
        get { return _myNumber; }
    }

    // 自身のエネルギー残量
    private int _myEnergy = 0;

    public int myEnergy
    {
        set {
            _myEnergy += value;
            if (_myEnergy > 9)
                _myEnergy = 9;
        }
        get { return _myEnergy; }
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

    // オーバーヒート
    private bool _isStan;

    public bool isStan
    {
        set { _isStan = value; }
        get { return _isStan; }
    }

    // 自身のAnimator
    Animator myAnim;

    // スタンエフェクトの一時保存用
    GameObject _stanEffect;

    // Use this for initialization
    void Start ()
    {
        stanEffect = Resources.Load("PlayerStan") as GameObject;
        pointPos = GetComponentInChildren<EffekseerEmitter>().gameObject.transform;
        myAudio = GetComponent<AudioSource>();
        myAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        myRig = GetComponent<Rigidbody>();
        // emitter = GetComponentInChildren<EffekseerEmitter>();

        for (int i = 0; i < GetPointArea().Length; i++)
        {
            targetList.Add(GetPointArea()[i]);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // ポーズ中は動かない
        if (Mathf.Approximately(Time.timeScale, 0.0f))
            return;

        if (isStan || MainManager.Instance.isStart == false)
            return;

        switch (state)
        {
            case ENEMY_STATE.PATROL:
                // 目標地点との距離が縮まったら
                float distance = Vector3.SqrMagnitude(transform.position - patrolPos);
                if (distance < 2.0f) {
                    // 巡回座標を初期化
                    patrolPos = Vector3.zero;
                }
                PatrolMove(patrolPos);
                SetTarget();
                break;
            case ENEMY_STATE.TARGETMOVE:
                // ターゲットがいるなら追従
                if (targetObj != null) {
                    Move(targetObj.transform.position);
                }
                // ターゲットがいないならパトロール
                else if(targetObj == null) {
                    state = ENEMY_STATE.PATROL;
                }
                break;
        }
        
	}

    /// <summary>
    /// ターゲットの状態を取得
    /// </summary>
    /// <param name="_targetObj">ターゲットオブジェクト</param>
    public override void CheckTarget(GameObject _targetObj)
    {
        if(_targetObj.GetComponent<Item>() != null)
        {
            // アイテムが入手不可能ならターゲット再設定
            if (targetObj.GetComponent<Item>().isCatch == false) {
                SetTarget();
                return;
            }
        }
        else if(_targetObj.GetComponentInParent<PointArea>() != null)
        {
            // ポイントエリアが機能していないならターゲット再設定
            if (targetObj.GetComponentInParent<PointArea>().isActive == false) {
                SetTarget();
                return;
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

        if (itemObj == null) {
            SearchTarget();
        }
        else {
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
            // 高得点アイテムを最優先
            if (GetItems()[i].point > 10 && GetTargetDistance(GetItems()[i].gameObject, gameObject) < minDistance) {
                if(GetItems()[i].isCarry == false)
                {
                    minDistance = GetTargetDistance(GetItems()[i].gameObject, gameObject);
                    targetObj = GetItems()[i].gameObject;
                }
                break;
            }

            // 最短距離のアイテムをターゲットに設定
            if (GetTargetDistance(GetItems()[i].gameObject, gameObject) < minDistance && GetItems()[i].isTarget == false) {
                // 最短距離の格納
                minDistance = GetTargetDistance(GetItems()[i].gameObject, gameObject);
                targetObj = GetItems()[i].gameObject;
            }
        }

        // ターゲットが設定されたらリターン
        if (minDistance != 100) {
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
        float[] averageDistance = new float[4];
        float[] enemyDistacne = new float[4];

        // すべてのポイントエリアにアクセス
        for (int i = 0; i < GetPointArea().Length; i++)
        {
            distance = GetTargetDistance(GetPointArea()[i].gameObject, gameObject);
            // 最短距離のポイントエリアをターゲットとする
            if (distance < minDistance && GetPointArea()[i].isActive == true) {
                minDistance = distance;
                targetObj = GetPointArea()[i].targetObj;
            }

            for (int j = 0; j < GetCharacter().Length; j++)
            {
                // 他のプレイヤーの方が近いならターゲットから除外
                enemyDistacne[j] = GetTargetDistance(GetPointArea()[i].gameObject, GetCharacter()[j]);
                if (minDistance > enemyDistacne[j] && GetCharacter()[j] != this) {
                    targetObj = null;
                }
                averageDistance[i] += enemyDistacne[j];
            }
            averageDistance[i] *= 0.3f;

            // 平均的に一番遠い位置へ移動
            if (averageDistance[i] > maxDistance && GetPointArea()[i].isActive == true) {
                maxDistance = averageDistance[i];
                dummyTarget = GetPointArea()[i].targetObj;
            }
        }

        // ターゲットが設定できたならリターン
        if (targetObj != null) {
            state = ENEMY_STATE.TARGETMOVE;
            return;
        }

        targetObj = dummyTarget;
    }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    public void Move(Vector3 vec)
    {
        CheckTarget(targetObj);

        if (_hasItem && myAnim.GetInteger("PlayAnimNum") != 11) {
            myAnim.SetInteger("PlayAnimNum", 11);
        }
        else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4) {
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
        if (myAnim.GetInteger("PlayAnimNum") != 4) {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // 巡回座標が初期化されていたら
        // 再度設定
        if (patrolPos == Vector3.zero) {
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
        if (isStan == true)
            return;

        LayerChange(2);

        myAudio.loop = true;
        AudioController.Instance.OtherAuioPlay(myAudio, "Stan");

        isStan = true;
        myRig.velocity = Vector3.zero;
        agent.updatePosition = false;

        myAnim.SetInteger("PlayAnimNum", 3);

        // スタンエフェクト生成
        _stanEffect = Instantiate(stanEffect, transform);
        _stanEffect.transform.localPosition = new Vector3(0.0f, 1.25f, 0.0f);

        // しばらく動けなくなる
        Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
        {
            agent.updatePosition = true;
            isStan = false;

            myAudio.loop = false;
            myAudio.Stop();

            _myEnergy = 0;
            // エナジーゲージの初期化
            HPCircle.Instance.EnergyReset(gameObject, _myNumber);
            Destroy(_stanEffect);

            // 2秒間無敵
            Observable.Timer(TimeSpan.FromSeconds(2.0f)).Subscribe(t =>
            {
                LayerChange(11);
            }).AddTo(this);

            SetTarget();
        }).AddTo(this);
    }

    /// <summary>
    /// アイテムの取得
    /// </summary>
    /// <param name="obj">アイテムのオブジェクト</param>
    public void Catch(GameObject obj)
    {
        if (hasItem == true || obj.GetComponent<Item>().isCatch == false)
            return;

        myRig.velocity = Vector3.zero;

        // アイテムを所持
        itemObj = obj;
        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);

        hasItem = true;
        LayerChange(12);
        SetTarget();
    }

    /// <summary>
    /// アイテムを放棄
    /// </summary>
    /// <param name="isSteal">アイテムを奪うかどうか</param>
    /// <param name="opponentPos">ぶつかってきたプレイヤーの座標</param>
    public void Release(bool isSteal, Vector3 opponentPos)
    {
        if (itemObj == null || hasItem == false) {
            ResetTarget();
            return;
        }
        
        AudioController.Instance.OtherAuioPlay(myAudio, "Release");

        myAnim.SetInteger("PlayAnimNum", 10);
        itemObj.GetComponent<Item>().ReleaseItem();
        hasItem = false;
        LayerChange(11);
        ResetTarget();
    }

    /// <summary>
    /// 荷物配達完了
    /// </summary>
    public void ItemCarry()
    {
        itemObj = null;
        hasItem = false;
        LayerChange(11);
    }

    public void LayerChange(int layerNum)
    {
        gameObject.layer = layerNum;
    }

    /// <summary>
    /// 巡回地点の取得
    /// </summary>
    /// <returns>ステージ上のランダム座標</returns>
    public override Vector3 GetRandomPosition()
    {
        // ステージのサイズ
        float stageSizeX = 10.0f, stageSizeZ = 8.0f;
        // 巡回用の座標を保存
        Vector3 nextPos = new Vector3(UnityEngine.Random.Range(-stageSizeX, stageSizeX), transform.position.y, UnityEngine.Random.Range(-stageSizeZ, stageSizeZ));
        return nextPos;
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
                return;

            character.Release(false, Vector3.zero);
        }
    }

    public override void OnCollisionExit(Collision col)
    {
        base.OnCollisionExit(col);
    }
}
