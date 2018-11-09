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
        set { _hasItem = value; }
        get { return _hasItem; }
    }

    // Use this for initialization
    void Start () {
        isCharge = false;
        pointPos = GetComponentInChildren<EffekseerEmitter>().gameObject.transform;
        myAnim = GetComponent<Animator>();
        myRig = GetComponent<Rigidbody>();
        system = FindObjectOfType<PlayerSystem>();
        emitter = GetComponentInChildren<EffekseerEmitter>();
	}
	
	// Update is called once per frame
	void Update () {
         PlayerInput();
	}

    // 入力判定
    void PlayerInput()
    {
        if (isAttack)
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
        if(system.ButtonUp_B(myNumber))
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
            return;

        if (_hasItem && myAnim.GetInteger("PlayAnimNum") != 11)
        {
            myAnim.SetInteger("PlayAnimNum", 11);
        }
        else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4)
        {
            myAnim.SetInteger("PlayAnimNum", 4);
        }

        // カメラの方向から、x-z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向の決定
        Vector3 moveForward = cameraForward * vec.z + Camera.main.transform.right * vec.x;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        if(isCharge == false)
        {
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
        if (isAttack)
            return;

        // エフェクト再生
        emitter.Play();

        myAnim.SetInteger("PlayAnimNum", 1);
        isAttack = true;
        isCharge = false;
        myAnim.SetInteger("PlayAnimNum", 8);

        //transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * _chargeLevel, 2.0f);
        myRig.AddForce(transform.forward * _chargeLevel * 300.0f, ForceMode.Acceleration);

        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(0.25f * _chargeLevel)).Subscribe(time =>
        {
            // チャージ段階を初期化
            _chargeLevel = 0;
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
        if(obj.GetComponent<Item>().isCatch == false)
        {
            return;
        }

        //myAnim.SetInteger("PlayAnimNum", 12);

        itemObj = obj;

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

        myAnim.SetInteger("PlayAnimNum", 10);
        itemObj.GetComponent<Item>().ReleaseItem(transform.position);
        itemObj = null;
        hasItem = false;
    }

    // パワーチャージ
    public void Charge()
    {
        if (isCharge)
            return;

        isCharge = true;

        _chargeLevel = 1;
        emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();

        var disposable = new SingleAssignmentDisposable();
        // 0.5秒ごとにチャージ
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(500)).Subscribe(time =>
        {
            // 3段階上昇、または攻撃で終了
            if (_chargeLevel >= 2 || isAttack)
            {
                disposable.Dispose();
            }

            // チャージ段階上昇
            _chargeLevel++;
            emitter.effectName = "Attack_Lv" + _chargeLevel.ToString();

        }).AddTo(this);
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
            myRig.velocity = Vector3.zero;

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
