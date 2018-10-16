using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Transform m_shootPoint = null;
    private Transform m_target = null;
    private GameObject m_shootObject = null;




    public void Shoot(Transform i_shootPoint, Transform i_targetPosition, GameObject i_shootObject)
    {
        m_shootPoint = i_shootPoint;
        m_target = i_targetPosition;
        m_shootObject = i_shootObject;
        // とりあえず適当に60度でかっ飛ばすとするよ！
        ShootFixedAngle(m_target.position, 60.0f);
    }

    private void ShootFixedAngle(Vector3 i_targetPosition, float i_angle)
    {
        //最初の速さベクトルを入手
        float speedVec = ComputeVectorFromAngle(i_targetPosition, i_angle);
        if (speedVec <= 0.0f)
        {
            // その位置に着地させることは不可能のようだ！
            Debug.LogWarning("!!");
            return;
        }
        //最初の速さベクトル　最初の角度　目標地点を送る
        //おそらく速さをもらっている？
        Vector3 vec = ConvertVectorToVector3(speedVec, i_angle, i_targetPosition);
        InstantiateShootObject(vec);
    }

    //最初の速さベクトルを計算
    private float ComputeVectorFromAngle(Vector3 i_targetPosition, float i_angle)
    {
        // xz平面の距離を計算。
        Vector2 startPos = new Vector2(m_shootPoint.transform.position.x, m_shootPoint.transform.position.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float x = distance;
        float g = Physics.gravity.y;
        float y0 = m_shootPoint.transform.position.y;
        float y = i_targetPosition.y;

        // Mathf.Cos()、Mathf.Tan()に渡す値の単位はラジアンだ。角度のまま渡してはいけないぞ！
        float rad = i_angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Square = g * x * x / (2 * cos * cos * (y - y0 - x * tan));

        // 負数を平方根計算すると虚数になってしまう。
        // 虚数はfloatでは表現できない。
        // こういう場合はこれ以上の計算は打ち切ろう。
        if (v0Square <= 0.0f)
        {
            return 0.0f;
        }

        float v0 = Mathf.Sqrt(v0Square);
        return v0;
    }
    
    //速さを返しているかな？ (もしかしたらｘ方向とｙ方向のベクトルを返しているのかも？)
    private Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition)
    {
        //おそらく発射位置
        Vector3 startPos = m_shootPoint.transform.position;
        //おそらく目標地点
        Vector3 targetPos = i_targetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        //目標に向かうための平面ベクトル 
        Vector3 dir = (targetPos - startPos).normalized;
        //開始位置から目標位置に向くための回転を求める
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        //最初の移動ベクトルかな？
        Vector3 vec = i_v0 * Vector3.right;
        //速さを求める　目標地点を向く角度×最初に飛ばす角度×最初の移動ベクトルかな？
        vec = yawRot * Quaternion.AngleAxis(i_angle, Vector3.forward) * vec;

        return vec;
    }

    //球の発射
    private void InstantiateShootObject(Vector3 i_shootVector)
    {
        //エラー回避
        if (m_shootObject == null)
        {
            throw new System.NullReferenceException("m_shootObject");
        }

        if (m_shootPoint == null)
        {
            throw new System.NullReferenceException("m_shootPoint");
        }

        var obj = Instantiate<GameObject>(m_shootObject, m_shootPoint.position, Quaternion.identity);
        var rigidbody = obj.AddComponent<Rigidbody>();

        // 速さベクトルのままAddForce()を渡してはいけないぞ。力(速さ×重さ)に変換するんだ
        Vector3 force = i_shootVector * rigidbody.mass;

        rigidbody.AddForce(force, ForceMode.Impulse);
    }
}
