using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// プレイヤークラス
/// </summary>
public class Player : PlayerBase, Character
{

    // インプット処理
    PlayerSystem system;

    // エフェクト再生
    EffekseerEmitter emitter;

    // 自身の番号(1 → 1P, 2 → 2P, 3 → 3P, 4 → 4P)
    public int _myNumber;

    public int myNumber
    {
        set { _myNumber = value; }
        get { return _myNumber; }
    }

    // 自身のエネルギー割合
    private int _myEnergy = 0;

    public int myEnergy
    {
        set { _myEnergy += value; }
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
        set { _hasItem = value; }
        get { return _hasItem; }
    }

    // オーバーヒート
    private bool _isStan;

    public bool isStan
    {
        set{ _isStan = value; }
        get { return _isStan; }
    }


    // チャージエフェクトの一時保存用
    GameObject _chargeEffect;
    // スタンエフェクトの一時保存用
    GameObject _stanEffect;
    // チャージエフェクト用マテリアル
    ParticleSystem.MainModule chargeMaterial;
    

    // Use this for initialization
    void Start () {
        chargeEffect = Resources.Load("Charge") as GameObject;
        stanEffect = Resources.Load("PlayerStan") as GameObject;
        emitter = GetComponentInChildren<EffekseerEmitter>();
        pointPos = emitter.gameObject.transform;
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
        if (isAttack || isStan)
            return;

        /* ここから移動量判定 */
        if (system.LeftStickAxis(myNumber) != Vector2.zero)
        {
            inputVec = new Vector3(system.LeftStickAxis(myNumber).x, 0, system.LeftStickAxis(myNumber).y);
            Move(inputVec);
        }
        else
        {
            myRig.velocity = Vector3.zero;
            if (_hasItem)
            {
                myAnim.SetInteger("PlayAnimNum", 11);
            }
            else if (myAnim.GetInteger("PlayAnimNum") != 8 && isAttack == false)
            {
                myAnim.SetInteger("PlayAnimNum", 8);
            }
        }

        if (system.Button_B(myNumber))
        {
            Charge();
        }
        if(system.ButtonUp_B(myNumber) && _chargeLevel != 0)
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

        // カメラの方向から、x-z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向の決定
        Vector3 moveForward = cameraForward * vec.z + Camera.main.transform.right * vec.x;

        if(isCharge == false)
        {
            if (_hasItem && myAnim.GetInteger("PlayAnimNum") != 11)
            {
                myAnim.SetInteger("PlayAnimNum", 11);
            }
            else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4)
            {
                myAnim.SetInteger("PlayAnimNum", 4);
            }

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            myRig.velocity = moveForward * runSpeed + new Vector3(0, myRig.velocity.y, 0);
        }
        else
        {
            myRig.velocity = Vector3.zero;
        }

        // キャラクターの向きを進行方向に
        transform.rotation = Quaternion.LookRotation(moveForward);

    }

    // タックル
    public void Attack()
    {
        if (isAttack || _chargeLevel == 0)
            return;

        if (_chargeEffect != null)
            Destroy(_chargeEffect);

        // エフェクト再生
        emitter.Play();

        myAnim.SetInteger("PlayAnimNum", 1);
        isAttack = true;
        isCharge = false;

        // エネルギー計算
        StartCoroutine(HPCircle.Instance.CheckOverHeat(gameObject, _myNumber, _chargeLevel));

        // チャージ段階に応じてアタック強化
        switch (_chargeLevel)
        {
            case 3:
                myRig.AddForce(transform.forward * (_chargeLevel - 1) * 200.0f, ForceMode.Acceleration);
                break;
            default:
                myRig.AddForce(transform.forward * _chargeLevel * 200.0f, ForceMode.Acceleration);
                break;
        }
        

        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(0.5f *  _chargeLevel)).Subscribe(time =>
        {
            myRig.velocity = Vector3.zero;
            myAnim.SetInteger("PlayAnimNum", 8);
            // チャージ段階を初期化
            _chargeLevel = 0;
            isAttack = false;

            // オーバーヒート
            if (_myEnergy >= 10) {
                Stan();
            }

        }).AddTo(this);
    }

    // ジャンプ
    public void Jump()
    {

    }

    // スタン
    public void Stan()
    {
        isStan = true;

        // スタンエフェクト生成
        _stanEffect = Instantiate(stanEffect, transform);
        _stanEffect.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);

        // しばらく動けなくなる
        Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
        {
            // エナジーゲージの初期化
            StartCoroutine(HPCircle.Instance.EnergyReset(gameObject, _myNumber));
            _myEnergy = 0;

            Destroy(_stanEffect);

            isStan = false;
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

        // チャージ中止
        isCharge = false;
        _chargeLevel = 0;
        Destroy(_chargeEffect);

        // アイテムを所持
        itemObj = obj;
        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);

        hasItem = true;
    }

    /// <summary>
    /// アイテムを放棄
    /// </summary>
    /// <param name="isSteal">アイテムを奪うかどうか</param>
    /// <param name="opponentPos">ぶつかってきたプレイヤーの座標</param>
    public void Release(bool isSteal, Vector3 opponentPos)
    {
        // アイテムを持っていないならリターン
        if (itemObj == null || hasItem == false)
        {
            itemObj = null;
            hasItem = false;
            return;
        }

        myAnim.SetInteger("PlayAnimNum", 10);
        itemObj.GetComponent<Item>().ReleaseItem(transform.position, transform.position, isSteal);
        itemObj = null;
        hasItem = false;
    }

    // パワーチャージ
    public void Charge()
    {
        // すでにチャージ中、アイテムを持っているならリターン
        if (isCharge || hasItem)
            return;

        // チャージ開始
        isCharge = true;
        _chargeLevel = 1;
        emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();

        // チャージエフェクト
        _chargeEffect = Instantiate(chargeEffect, transform);
        _chargeEffect.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        chargeMaterial = _chargeEffect.GetComponent<ParticleSystem>().main;

        var disposable = new SingleAssignmentDisposable();
        // 0.5秒ごとにチャージ
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(750)).Subscribe(time =>
        {
            // 3段階上昇、または攻撃で終了
            if (_chargeLevel >= 2 || isAttack)
            {
                disposable.Dispose();
            }

            // チャージ段階上昇
            _chargeLevel++;
            emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();

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

            // アイテムを登録
            GameObject itemObj = col.gameObject.GetComponentInChildren<Item>().gameObject;
            // チャージが最大レベルなら
            if (_chargeLevel == 3)
            {
                // アイテム放棄
                character.Release(true, transform.position);
                return;
            }

            character.Release(false, Vector3.zero);
        }
    }
}
