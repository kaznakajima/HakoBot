using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Item : MonoBehaviour
{
    // 自身のCollider
    Collider myCol;

    // 自身のRigidbbody
    Rigidbody myRig;

    // 取得できるかどうか
    [HideInInspector]
    public bool isCatch;

    // ターゲットにされているか
    [HideInInspector]
    public bool isTarget;

    // コンベアに乗ったかどうか
    [HideInInspector]
    public bool isCarry;

    // 目標地点
    [HideInInspector]
    public Vector3 throwPos = Vector3.zero;

    // 移動方向
    Vector3 dir;

    // 得点
    public int point;

	// Use this for initialization
	void Start () {
        isCatch = true;
        isTarget = false;
        myCol = GetComponent<Collider>();
        myRig = GetComponent<Rigidbody>();

        // 1秒後に移動再開
        //Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(time => {
        //    ReleaseItem();
        //}).AddTo(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (isCarry)
            Move(dir);
	}

    /// <summary>
    /// コンベアに乗せた後の処理
    /// </summary>
    /// <param name="parent">乗せたコンベアのオブジェクト</param>
    /// <param name="dir">方向</param>
    public void ItemCarry(GameObject parent, Vector3 _dir)
    {
        // 向きを取得
        dir = _dir;

        // 位置の調整
        transform.parent = parent.transform;
        transform.rotation = Quaternion.identity;
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.2f);

        // コンベアを流れていく
        isCarry = true;
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="dir">方向</param>
    void Move(Vector3 dir)
    {
        transform.position += dir * 2.0f * Time.deltaTime;
    }

    /// <summary>
    /// プレイヤーに所持される
    /// </summary>
    /// <param name="point">持っている位置</param>
    public void GetItem(Transform point)
    {
        if(isCatch == false)
            return;

        // プレイヤーの取得位置に配置
        transform.position = point.position;
        // 向きを修正
        transform.rotation = point.rotation;
        isCatch = false;
        myCol.isTrigger = true;
        myRig.useGravity = false;

        // 動き、向きを固定
        myRig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationY;
    }

    /// <summary>
    /// 取得状態から放棄する
    /// </summary>
    public void ReleaseItem()
    {
        gameObject.layer = 10;
        transform.parent = null;
        myCol.isTrigger = false;
        myRig.useGravity = true;
        myRig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // 目標地点
        throwPos = new Vector3(UnityEngine.Random.Range(-15.0f, 15.0f), 0.5f, UnityEngine.Random.Range(-7.0f, 7.1f));
        // 射出角度、方向を取得
        float angle = 30.0f;
        Vector3 velocity = CalculateVeclocity(transform.position, throwPos, angle);
        // 射出
        myRig.AddForce(velocity * myRig.mass, ForceMode.Impulse);
    }

    /// <summary>
    /// オブジェクトを飛ばす座標の計算
    /// </summary>
    /// <param name="pointA">初期座標</param>
    /// <param name="pointB">目標座標</param>
    /// <param name="angle">打ち出す角度</param>
    /// <returns></returns>
    public Vector3 CalculateVeclocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "st")
        {
            gameObject.layer = 9;
            isCatch = true;
            isTarget = false;
        }
        if(col.gameObject.tag == "Pawn") {
            ReleaseItem();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.GetComponent<PointArea>() != null)
        {
            Destroy(gameObject);
        }
    }
}
