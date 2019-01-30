﻿using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class BalanceEnemy : EnemyBase, Character
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

            if (_hasItem == false)
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
    void Start()
    {
        stanEffect = Resources.Load("PlayerStan") as GameObject;
        emitter = GetComponentInChildren<EffekseerEmitter>();
        pointPos = emitter.gameObject.transform;
        myAudio = GetComponent<AudioSource>();
        myAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        myRig = GetComponent<Rigidbody>();
        emitter.effectName = "Attack";
        layerNum = gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {
        // ポーズ中は動かない
        if (Mathf.Approximately(Time.timeScale, 0.0f))
            return;
        
        // オーバーヒート中はリターン
        if (MainManager.Instance.isStart == false || isStan)
            return;

        AI_Move();
    }

    /// <summary>
    /// AIを動かせる(Update)
    /// </summary>
    public override void AI_Move()
    {
        // AIの状態によって分岐
        switch (state)
        {
            // パトロール
            case ENEMY_STATE.PATROL:
                PatrolMove(patrolPos);
                SetTarget();
                break;
            // ターゲット追従
            case ENEMY_STATE.TARGETMOVE:
                // ターゲットがいるなら追従
                if (targetObj != null)
                {
                    Move(targetObj.transform.position);
                }
                // ターゲットを見失ったらパトロール
                else if (targetObj == null)
                {
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
        if (_targetObj.GetComponent<Item>() != null)
        {
            // アイテムが入手不可能ならターゲット再設定
            if (_targetObj.GetComponent<Item>().isCatch == false) {
                SetTarget();
                return;
            }
        }
        else if (_targetObj.GetComponentInParent<PointArea>() != null)
        {
            // ポイントエリアが機能していないならターゲット再設定
            if (_targetObj.GetComponentInParent<PointArea>().isActive == false) {
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
        // ターゲットリセット
        if (targetObj != null && targetObj.GetComponentInParent<PointArea>() != null) {
            targetObj.GetComponentInParent<PointArea>().isTarget = false;
        }

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
        // 荷物を持っていないなら
        if (itemObj == null) {
            // アイテム、敵探索
            SearchTarget();
        }
        // 荷物を持っているなら
        else {
            // 得点エリア探索
            SearchPointArea();
        }
    }

    /// <summary>
    /// アイテム、敵を探索し、ターゲットにする
    /// </summary>
    public override void SearchTarget()
    {
        // アイテムをリスト化
        var targetList = GetItems().Where(obj =>obj.isCatch == true && obj.isTarget == false)
            .OrderBy(obj => GetTargetDistance(obj.gameObject, gameObject)).ToList();

        targetList.OrderByDescending(obj => obj.point).ToList();

        // 最短距離のアイテムをターゲット指定
        foreach(Item item in targetList) {
            targetObj = item.gameObject;
            targetObj.GetComponent<Item>().isTarget = true;
            state = ENEMY_STATE.TARGETMOVE;
            break;
        }

        // 敵をリスト化
        var enemyTarget = GetCharacter().OrderBy(obj => GetTargetDistance(obj, gameObject)).ToList();

        // 最短距離の敵をターゲット指定
        foreach (GameObject obj in enemyTarget) {
            if (obj != gameObject)
            {
                var character = obj.GetComponent(typeof(Character)) as Character;
                // チーム戦の場合、見方は除外
                if (MainManager.Instance.playerData[character.myNumber - 1].m_Team 
                    == MainManager.Instance.playerData[myNumber - 1].m_Team) return;

                // 荷物を持っているならターゲット指定
                if (character.hasItem == true)
                {
                    targetObj = obj;
                    state = ENEMY_STATE.TARGETMOVE;
                    break;
                }
            } 
        }

        // ターゲットがいないならパトロール
        if (targetObj == null) state = ENEMY_STATE.PATROL;
    }

    /// <summary>
    /// ポイントエリアを探索し、ターゲットにする
    /// </summary>
    public override void SearchPointArea()
    {
        // ポイントエリアのリスト化
        var targetList = GetPointArea().Where(obj => obj.isActive == true && obj.isTarget == false)
            .OrderBy(obj => GetTargetDistance(obj.gameObject, gameObject)).ToList();

        foreach(PointArea area in targetList) {
            targetObj = area.gameObject;
            targetObj.GetComponent<PointArea>().isTarget = true;
            state = ENEMY_STATE.TARGETMOVE;
            break;
        }
    }

    /// <summary>
    /// AIの移動
    /// </summary>
    /// <param name="vec">目標地点</param>
    public void Move(Vector3 vec)
    {
        CheckTarget(targetObj);

        if (_hasItem && myAnim.GetInteger("PlayAnimNum") != 11) myAnim.SetInteger("PlayAnimNum", 11);
        else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4) myAnim.SetInteger("PlayAnimNum", 4);

        // ターゲットがアイテムを持っていないならターゲット変更
        if (targetObj.tag == "Character")
        {
            var character = targetObj.GetComponent(typeof(Character)) as Character;

            // ターゲットが荷物を失ったらターゲット再指定
            if (character.hasItem == false) {
                SetTarget();
                return;
            }

            // 攻撃範囲に入ったら攻撃
            if (GetTargetDistance(targetObj, gameObject) < 6.0f && isAttack == false) Attack();
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

        agent.SetDestination(vec);
    }

    /// <summary>
    /// 巡回移動
    /// </summary>
    /// <param name="vec">目標地点</param>
    public override void PatrolMove(Vector3 vec)
    {
        if (myAnim.GetInteger("PlayAnimNum") != 4) myAnim.SetInteger("PlayAnimNum", 4);

        // 目標地点との距離が縮まったら
        float distance = Vector3.SqrMagnitude(transform.position - patrolPos);
        // 巡回座標を初期化
        if (distance < 2.0f) patrolPos = Vector3.zero;

        // 巡回座標が初期化されていたら
        // 再度設定
        if (patrolPos == Vector3.zero) GetRandomPosition();

        agent.SetDestination(vec);
    }

    /// <summary>
    /// タックル攻撃
    /// </summary>
    public void Attack()
    {
        // 連続攻撃しない
        if (isAttack) return;

        // エフェクト再生
        emitter.Play("Attack_Lv1");

        // エネルギー計算
        HPCircle.Instance.CheckOverHeat(gameObject, _myNumber);

        myAnim.SetInteger("PlayAnimNum", 1);
        isAttack = true;

        // アタック
        myRig.velocity += transform.forward * 7.5f;

        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(time =>
        {
            myAnim.SetInteger("PlayAnimNum", 8);
            myRig.velocity = Vector3.zero;

            // 移動制限解除
            isAttack = false;

            // オーバーヒート
            if (_myEnergy >= 9) Stan("Stan");
            // 攻撃終了後ターゲット探索
            else SetTarget();

        }).AddTo(this);
    }

    public void Jump()
    {

    }

    public void Stan(string audioStr)
    {
        if (isStan == true || _stanEffect != null) return;

        LayerChange(2);
        if(audioStr == "Stan") {
            myAudio.loop = true;
            AudioController.Instance.OtherAuioPlay(myAudio, audioStr);
        }

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

            if(audioStr == "Stan") {
                myAudio.loop = false;
                myAudio.Stop();
            }

            _myEnergy = 0;
            // エナジーゲージの初期化
            HPCircle.Instance.EnergyReset(gameObject, _myNumber);
            Destroy(_stanEffect);

            isStan = false;

            // 2秒間無敵
            Observable.Timer(TimeSpan.FromSeconds(2.0f)).Subscribe(t =>
            {
                LayerChange(layerNum);
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
        if (hasItem == true || obj.GetComponent<Item>().isCatch == false) return;

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
        LayerChange(layerNum);
        ResetTarget();
    }

    /// <summary>
    /// 荷物配達完了
    /// </summary>
    public void ItemCarry()
    {
        // ターゲットリセット
        targetObj.GetComponentInParent<PointArea>().isTarget = false;

        itemObj = null;
        hasItem = false;
        LayerChange(layerNum);
    }

    /// <summary>
    /// レイヤー変更
    /// </summary>
    /// <param name="layerNum">レイヤー番号</param>
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
        if (col.gameObject.tag == "Item") Catch(col.gameObject);

        // タックル中にプレイヤーに触れたとき
        if (col.gameObject.GetComponent(typeof(Character)) as Character != null && isAttack)
        {
            myRig.velocity = Vector3.zero;

            var character = col.gameObject.GetComponent(typeof(Character)) as Character;

            // 同じチームだったらリターン
            if (MainManager.Instance.playerData[character.myNumber - 1].m_Team ==
                MainManager.Instance.playerData[myNumber - 1].m_Team) return;

            // 触れたプレイヤーがアイテムを持っていないならリターン
            if (character.hasItem == false) return;

            AudioController.Instance.OtherAuioPlay(myAudio, "Damage");

            character.Release(false, Vector3.zero);
        }
    }

    public override void OnCollisionExit(Collision col)
    {
        base.OnCollisionExit(col);
    }
}
