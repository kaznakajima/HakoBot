﻿using System.Collections;
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

    // オーバーヒート
    private bool _isStan;

    public bool isStan
    {
        set { _isStan = value; }
        get { return _isStan; }
    }

    // 自身のAnimator
    Animator myAnim;

    // チャージエフェクトの一時保存用
    GameObject _chargeEffect;
    // スタンエフェクトの一時保存用
    GameObject _stanEffect;
    // チャージエフェクト用マテリアル
    ParticleSystem.MainModule chargeMaterial;
   

    // Use this for initialization
    void Start()
    {
        chargeEffect = Resources.Load("Charge") as GameObject;
        stanEffect = Resources.Load("PlayerStan") as GameObject;
        emitter = GetComponentInChildren<EffekseerEmitter>();
        pointPos = emitter.gameObject.transform;
        myAudio = GetComponent<AudioSource>();
        myAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        myRig = GetComponent<Rigidbody>();
        emitter.effectName = "Attack";

        for(int i = 0;i < GetPointArea().Length; i++)
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

        if (MainManager.Instance.isStart == false)
            return;

        // オーバーヒート中はリターン
        if (isStan || isAttack)
            return;

        switch (state)
        {
            case ENEMY_STATE.PATROL:
                // 目標地点との距離が縮まったら
                float distance = Vector3.SqrMagnitude(transform.position - patrolPos);
                if(distance < 2.0f) {
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
                else if (targetObj == null) {
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
            if (targetObj.GetComponent<Item>().isCatch == false) {
                SetTarget();
                return;
            }
        }
        else if (_targetObj.GetComponentInParent<PointArea>() != null)
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
    /// 他のキャラクターとの距離を取得
    /// </summary>
    public override void SearchTarget()
    {
        // ステージ上のすべてのプレイヤーにアクセス
        for (int i = 0; i < GetCharacter().Length; i++)
        {
            // 最短距離のプレイヤーをターゲット設定
            if (GetTargetDistance(gameObject, GetCharacter()[i]) < minDistance)
            {
                var character = GetCharacter()[i].GetComponent(typeof(Character)) as Character;
                if (character.hasItem == true && GetCharacter()[i] != gameObject) {
                    // 最短距離の格納
                    minDistance = GetTargetDistance(gameObject, GetCharacter()[i]);
                    targetObj = GetCharacter()[i];
                }
            }
        }

        // ターゲットが設定されたらリターン
        if (minDistance != 100) {
            state = ENEMY_STATE.TARGETMOVE;
            return;
        }

        // 誰もアイテムを所持していないなら
        // ステージ上のアイテムすべてにアクセス
        for (int i = 0; i < GetItems().Length; i++)
        {
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

        // それでもターゲットが設定できないなら巡回
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

        // ターゲットがアイテムを持っていないならターゲット変更
        if (targetObj.tag == "Character")
        {
            var character = targetObj.GetComponent(typeof(Character)) as Character;
            if (character.hasItem == false) {
                SetTarget();
                return;
            }

            // 攻撃範囲に入ったら攻撃
            if (GetTargetDistance(targetObj, gameObject) < 6.0f) {
                Attack();
            }
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
        if (myAnim.GetInteger("PlayAnimNum") != 4) {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // 巡回座標が初期化されていたら
        // 再度設定
        if(patrolPos == Vector3.zero) {
            GetRandomPosition();
        }

        agent.SetDestination(vec);
    }

    /// <summary>
    /// タックル攻撃
    /// </summary>
    public void Attack()
    {

        if (_chargeEffect != null)
            Destroy(_chargeEffect);

        // エフェクト再生
        emitter.Play();

        // エネルギー計算
        StartCoroutine(HPCircle.Instance.CheckOverHeat(gameObject, _myNumber, _chargeLevel));

        myAnim.SetInteger("PlayAnimNum", 1);
        isAttack = true;

        // チャージ段階に応じてアタック強化
        switch (_chargeLevel)
        {
            case 3:
                //myRig.AddForce(transform.forward * (_chargeLevel - 1) * 200.0f, ForceMode.Acceleration);
                myRig.velocity = transform.forward * 5.0f * _chargeLevel;
                break;
            default:
                //myRig.AddForce(transform.forward * _chargeLevel * 200.0f, ForceMode.Acceleration);
                myRig.velocity = transform.forward * 10.0f;
                break;
        }


        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(time =>
        {
            myAnim.SetInteger("PlayAnimNum", 8);
            // チャージ段階を初期化
            _chargeLevel = 0;
            myRig.velocity = Vector3.zero;

            // 移動制限解除
            isCharge = false;
            isAttack = false;

            // オーバーヒート
            if (_myEnergy >= 9) {
                Stan();
            }
            else {
                SetTarget();
            }
        }).AddTo(this);
    }

    public void Jump()
    {

    }

    public void Stan()
    {
        myAudio.loop = true;
        AudioController.Instance.OtherAuioPlay(myAudio, "Stan");

        isStan = true;

        // スタンエフェクト生成
        _stanEffect = Instantiate(stanEffect, transform);
        _stanEffect.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);

        // しばらく動けなくなる
        Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
        {
            myAudio.loop = false;
            myAudio.Stop();

            _myEnergy = 0;
            // エナジーゲージの初期化
            StartCoroutine(HPCircle.Instance.EnergyReset(gameObject, _myNumber));

            Destroy(_stanEffect);

            isStan = false;
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

        //// チャージ中止
        //isCharge = false;
        //agent.updatePosition = true;
        //_chargeLevel = 0;
        Destroy(_chargeEffect);

        // アイテムを所持
        itemObj = obj;
        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);

        hasItem = true;
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
        itemObj.GetComponent<Item>().ReleaseItem(transform.position, opponentPos, isSteal);
        hasItem = false;
        ResetTarget();
    }

    /// <summary>
    /// パワーチャージ
    /// </summary>
    public void Charge()
    {
        if (_chargeLevel != 0 || hasItem)
            return;

        // 移動制限
        isCharge = true;
        agent.updatePosition = false;

        // チャージ開始
        _chargeLevel = 1;
        //emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();
        emitter.effectName = "Attack";

        // チャージエフェクト生成
        _chargeEffect = Instantiate(chargeEffect, transform);
        _chargeEffect.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        chargeMaterial = _chargeEffect.GetComponent<ParticleSystem>().main;

        var disposable = new SingleAssignmentDisposable();
        // 1.0秒ごとにチャージ
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(750)).Subscribe(time =>
        {
            // 3段階上昇、または攻撃で終了
            if (_chargeLevel >= 3 || isAttack) {
                Attack();
                disposable.Dispose();
            }
            else if (_chargeLevel == 0) {
                Destroy(_chargeEffect);
                disposable.Dispose();
            }

            // チャージ段階上昇
            _chargeLevel++;
            emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();

            // エフェクトが生成しきれていないならリターン
            if (_chargeEffect == null)
                return;

            // チャージ段階に応じてエフェクトの見た目変更
            switch (_chargeLevel)
            {
                case 1:
                    chargeMaterial.startColor = Color.white;
                    break;
                case 2:
                    chargeMaterial.startColor = Color.yellow;
                    break;
                case 3:
                    chargeMaterial.startColor = Color.red;
                    break;
            }

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
        if (col.gameObject.tag == "Item")
        {
            Catch(col.gameObject);
        }

        // タックル中にプレイヤーに触れたとき
        if (col.gameObject.GetComponent(typeof(Character)) as Character != null && isAttack)
        {

            myRig.velocity = Vector3.zero;

            var character = col.gameObject.GetComponent(typeof(Character)) as Character;

            // 触れたプレイヤーがアイテムを持っていないならリターン
            if (character.hasItem == false)
                return;

            AudioController.Instance.OtherAuioPlay(myAudio, "Damage");

            character.Release(false, Vector3.zero);
        }
    }

    public override void OnCollisionExit(Collision col)
    {
        base.OnCollisionExit(col);
    }
}
