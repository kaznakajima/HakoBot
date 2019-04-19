using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using XInputDotNetPure;

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

    // 自身のインプットステート
    public int myInputState;

    // 自身のエネルギー割合
    private int _myEnergy = 0;

    public int myEnergy
    {
        set {
            _myEnergy += value;
            if (_myEnergy > 9) _myEnergy = 9;
        }
        get { return _myEnergy; }
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

    // ターゲットとされているか
    private bool _isTarget;
    public bool isTarget
    {
        set { _isTarget = value; }
        get { return _isTarget; }
    }

    // スタンエフェクトの一時保存用
    GameObject _stanEffect;

    // Use this for initialization
    void Start ()
    {
        stanEffect = Resources.Load("PlayerStan") as GameObject;
        emitter = GetComponentInChildren<EffekseerEmitter>();
        emitter.effectName = "Attack";
        pointPos = emitter.gameObject.transform;
        myAudio = GetComponent<AudioSource>();
        myAnim = GetComponent<Animator>();
        flashAnim = GetComponentsInChildren<Animator>()
            .Where(obj => obj.GetComponent<SkinnedMeshRenderer>() != null).FirstOrDefault();
        myRig = GetComponent<Rigidbody>();
        system = FindObjectOfType<PlayerSystem>();
        layerNum = gameObject.layer;
	}
	
	// Update is called once per frame
	void Update () {
        // ポーズ中は動かない
        if (Mathf.Approximately(Time.timeScale, 0.0f) || MainManager.Instance.isStart == false) return;

        // スタン中は動かない
        if (isStan) return;

        PlayerInput();
	}

    // 入力判定
    void PlayerInput()
    {
        if (isAttack) return;

        /* ここから移動量判定 */
        if(myInputState == 0 && system.KeyboardAxis() != Vector2.zero) {
            inputVec = new Vector3(system.KeyboardAxis().x, 0, system.KeyboardAxis().y);
            Move(inputVec);
        }
        else if(myInputState == 0 && system.KeyboardAxis() == Vector2.zero) {
            myRig.velocity = Vector3.zero;
            if (hasItem) myAnim.SetInteger("PlayAnimNum", 11);
            else if (myAnim.GetInteger("PlayAnimNum") != 8 && isAttack == false) myAnim.SetInteger("PlayAnimNum", 8);
        }
        // ゲームパッド
        if (myInputState != 0 && system.LeftStickAxis(myInputState) != Vector2.zero) {
            inputVec = new Vector3(system.LeftStickAxis(myInputState).x, 0, system.LeftStickAxis(myInputState).y);
            Move(inputVec);
        }
        else if(myInputState != 0 && system.LeftStickAxis(myInputState) == Vector2.zero)
        {
            myRig.velocity = Vector3.zero;
            if (hasItem) myAnim.SetInteger("PlayAnimNum", 11);
            else if (myAnim.GetInteger("PlayAnimNum") != 8 && isAttack == false) myAnim.SetInteger("PlayAnimNum", 8);
        }

        if (myInputState == 0 && system.Keyboard_X()) Attack();
        else if (myInputState != 0 && system.Button_B(myInputState)) Attack();
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

        if (_hasItem && myAnim.GetInteger("PlayAnimNum") != 11) myAnim.SetInteger("PlayAnimNum", 11);
        else if (!_hasItem && myAnim.GetInteger("PlayAnimNum") != 4) myAnim.SetInteger("PlayAnimNum", 4);

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        myRig.velocity = moveForward.normalized * runSpeed + new Vector3(0, myRig.velocity.y, 0);

        // キャラクターの向きを進行方向に
        transform.rotation = Quaternion.LookRotation(moveForward);

    }

    // タックル
    public void Attack()
    {
        if (hasItem) return;

        // エフェクト再生
        emitter.Play("Attack_Lv1");

        myAnim.SetInteger("PlayAnimNum", 1);
        isAttack = true;

        // エネルギー計算
        HPCircle.Instance.CheckOverHeat(gameObject, _myNumber);

        myRig.velocity += transform.forward * 8.5f;

        // 1秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(time =>
        {
            myRig.velocity = Vector3.zero;
            myAnim.SetInteger("PlayAnimNum", 8);
            isAttack = false;

            // オーバーヒート
            if (_myEnergy >= 9) Stan("Stan");

        }).AddTo(this);
    }

    // スタン
    public void Stan(string audioStr)
    {
        if (isStan == true || _stanEffect != null) return;

        LayerChange(2);

        if (audioStr == "Stan") {
            myAudio.loop = true;
            AudioController.Instance.OtherAuioPlay(myAudio, audioStr);
        }

        isStan = true;
        myRig.velocity = Vector3.zero;

        myAnim.SetInteger("PlayAnimNum", 3);

        // スタンエフェクト生成
        _stanEffect = Instantiate(stanEffect, transform);
        _stanEffect.transform.localPosition = new Vector3(0.0f, 1.25f, 0.0f);

        // しばらく動けなくなる
        Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
        {
            flashAnim.SetBool("IsFlash_p" + myNumber, true);

            if (audioStr == "Stan") {
                myAudio.loop = false;
                myAudio.Stop();
            }

            _myEnergy = 0;
            // エナジーゲージの初期化
            HPCircle.Instance.EnergyReset(gameObject, _myNumber);
            Destroy(_stanEffect);

            // 2秒間無敵
            Observable.Timer(TimeSpan.FromSeconds(2.0f)).Subscribe(t =>
            {
                flashAnim.SetBool("IsFlash_p" + myNumber, false);
                isStan = false;
                LayerChange(layerNum);
            }).AddTo(this);

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

        // アイテムを所持
        itemObj = obj;
        itemObj.transform.parent = transform;
        itemObj.GetComponent<Item>().GetItem(pointPos);
        LayerChange(12);

        hasItem = true;
    }

    /// <summary>
    /// アイテムを放棄
    /// </summary>
    public void Release()
    {
        // アイテムを持っていないならリターン
        if (itemObj == null || hasItem == false) {
            itemObj = null;
            hasItem = false;
            return;
        }

        VibrationController.Instance.PlayVibration(myNumber - 1, true);
        AudioController.Instance.OtherAuioPlay(myAudio, "Release");

        myAnim.SetInteger("PlayAnimNum", 10);
        itemObj.GetComponent<Item>().ReleaseItem();
        itemObj = null;
        hasItem = false;
        LayerChange(layerNum);
    }

    /// <summary>
    /// 荷物配達完了
    /// </summary>
    public void ItemCarry()
    {
        itemObj = null;
        hasItem = false;
        LayerChange(layerNum);
    }

    /// <summary>
    /// レイヤー変更
    /// </summary>
    /// <param name="layerNum">インデックス番号</param>
    public void LayerChange(int layerNum)
    {
        gameObject.layer = layerNum;
    }

    // 当たり判定
    void OnCollisionEnter(Collision col)
    {
        // アイテムだったらアイテム取得
        if (col.gameObject.tag == "Item") Catch(col.gameObject);

        // タックル中にプレイヤーに触れたとき
        var character = col.gameObject.GetComponent<Character>();
        if (character != null && isAttack)
        {
            myRig.velocity = Vector3.zero;

            // 同じチームだったらリターン
            if (MainManager.Instance.playerData[character.myNumber - 1].m_Team ==
                MainManager.Instance.playerData[myNumber - 1].m_Team) return;

            // 触れたプレイヤーがアイテムを持っていないならリターン
            if (character.hasItem == false) return;

            AudioController.Instance.OtherAuioPlay(myAudio, "Damage");

            character.Release();
        }
    }
}
