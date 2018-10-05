using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    // 自身のCollider
    Collider myCol;

    // 自身のRigidbbody
    Rigidbody myRig;

    // 取得できるかどうか
    [HideInInspector]
    public bool isCatch;

	// Use this for initialization
	void Start () {
        isCatch = true;
        myCol = GetComponent<Collider>();
        myRig = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// プレイヤーに所持される
    /// </summary>
    /// <param name="point">持っている位置</param>
    public void GetItem(Transform point)
    {
        if(isCatch == false)
        {
            return;
        }

        transform.position = point.position;
        isCatch = false;
        myCol.isTrigger = true;
        myRig.useGravity = false;
        myRig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    /// <summary>
    /// 取得状態から放棄する
    /// </summary>
    /// <param name="playerPos">取得しているプレイヤー座標</param>
    public void ReleaseItem(Vector3 playerPos)
    {
        myRig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        myRig.useGravity = true;

        // 目標地点
        Vector3 throwPos = new Vector3(Random.Range(-4.0f, 4.0f), 0.0f, Random.Range(-4.0f, 4.0f));
        // 射出角度、方向を取得
        float angle = 60.0f;
        Vector3 velocity = CalculateVeclocity(transform.position, throwPos, angle);

        // 射出
        myRig.AddForce(velocity * myRig.mass, ForceMode.Impulse);

        transform.parent = null;
        myCol.isTrigger = false;
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
        if(col.gameObject.name == "Plane")
        {
            isCatch = true;
        }
    }
}
