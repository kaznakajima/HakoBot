using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poibot_Throwing : MonoBehaviour
{


    //投てき物 通常荷物　高得点荷物
    [SerializeField]
    private GameObject baggage;
    [SerializeField]
    private GameObject highPointBaggage;

    //投てき開始位置と目標位置
    [SerializeField]
    private Transform startPoint;
    private Vector3 targetPoint;
    // 射出角度
    private float angle = 30.0f;
    //投てき範囲（現在は正方形バージョン）
    [SerializeField]
    private float range;

    public bool playEvent = false;

    // 投げるロボット
    [SerializeField]
    ThrowEnemy th_Enemy;

    public void ThrowingsetUp(System.Action action)
    {
        targetPoint = SearchThrowingTarget();
        Throwing();
        action();
    }

    /// <summary>
    /// 投てき目標地点の決定
    /// </summary>
    private Vector3 SearchThrowingTarget()
    {
        var posx = Random.Range(-range, range);
        var posz = Random.Range(-range, range);
        Vector3 pos = new Vector3(posx, 0.5f, posz);
        return pos;
    }

    /// <summary>
    /// 投てきを開始させる
    /// </summary>
    private void Throwing()
    {
        var throwingObj = baggage;

        if (playEvent)
        {
            var t = Random.Range(0, 5);
            if (t == 0) throwingObj = highPointBaggage;
        }

        var obj = Instantiate(throwingObj, transform.position, Quaternion.identity) as GameObject;
        th_Enemy.item = obj.GetComponent<Item>();
        th_Enemy.Throw();
        // 標的の座標
        //Vector3 targetPosition = targetPoint;

        // 射出速度を算出
        //Vector3 velocity = CalculateVelocity(transform.position, targetPosition, angle);

        // 射出
        //Rigidbody rid = obj.GetComponent<Rigidbody>();
        //rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
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
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        else
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
    }
}
