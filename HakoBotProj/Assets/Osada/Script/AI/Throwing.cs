using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UniRx;

public class Throwing : MonoBehaviour
{
    // 投てきするオブジェクト
    private GameObject throwingObject;
    // 標的のオブジェクト
    private GameObject targetObject;
    // 射出角度
    private float ThrowingAngle;

    /// <summary>
    /// 投てきを開始させる
    /// </summary>
    public void ThrowingStart()
    {
        if (throwingObject == null || targetObject == null) return;

        GameObject obj = Instantiate(throwingObject, this.transform.position, Quaternion.identity);
        // 標的の座標
        Vector3 targetPosition = targetObject.transform.position;

        // 射出角度
        float angle = ThrowingAngle;

        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(this.transform.position, targetPosition, angle);

        // 射出
        Rigidbody rid = obj.GetComponent<Rigidbody>();
        rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
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
