using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class AI_Poibot : MonoBehaviour
{
    //移動方向
    public enum Direction
    {
        vertical,   //縦移動
        Horizontal, //横移動
        All         //全方向
    }
    public Direction direction;
    //移動範囲
    private float moveRange;
    //移動速度
    private float speed;
    //移動中かの判断
    private BoolReactiveProperty actionEnd = new BoolReactiveProperty(false);
    //移動開始位置と目標位置
    private Vector3 startPos, endPos;
    //移動開始時間保存用
    private float startTime = 0.0f;

    //投てき物
    private GameObject throwingObject;
    //投てき発射位置と目標位置
    private Vector3 targetPoint;
    // 射出角度
    private float throwingAngle;
    //投てき範囲（現在は正方形バージョン）
    private float throwingRange;

    private void Start()
    {
        //移動開始
        actionEnd.Where(c => !c)
            .Subscribe(c =>
            {
                //開始位置と目標位置を設定
                startPos = transform.position;
                endPos = SearchMoveTarget();
                //開始時間を保存
                startTime = Time.timeSinceLevelLoad;

                StartCoroutine("Move");
            }).AddTo(this);

        //荷物投げ開始
        actionEnd.Where(c => c)
            .Subscribe(c =>
            {
                //荷物を投げさせる
                ThrowingStart();
                //荷物投げ完了、移動を開始
                actionEnd.Value = false;
            }).AddTo(this);
    }

    //移動目標地点の検索
    private Vector3 SearchMoveTarget()
    {
        var t = Random.Range(-moveRange, moveRange);
        Vector3 pos = transform.position;
        switch (direction)
        {
            case Direction.Horizontal:
                pos.x = t;
                break;
            case Direction.vertical:
                pos.z = t;
                break;
        }
        return pos;
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move()
    {
        //投げるまでの時間を設定
        var time = Random.Range(3.0f, 5.0f);

        while (true)
        {
            //現在の経過時間
            var diff = Time.timeSinceLevelLoad - startTime;
            //割合計算
            var rate = diff / time;

            if (rate <= 1)
            {
                //移動処理
                transform.position = Vector3.Lerp(startPos, endPos, rate);
                yield return null;
            }
            else
            {
                //移動完了、荷物投げに移行
                actionEnd.Value = true;
                break;
            }
        }
    }

    /// <summary>
    /// 投てき目標地点の決定
    /// </summary>
    private void SearchThrowingTarget()
    {

    }

    /// <summary>
    /// 投てきを開始させる
    /// </summary>
    private void ThrowingStart()
    {
        if (targetPoint == null || throwingObject == null) return;

        GameObject obj = Instantiate(throwingObject, this.transform.position, Quaternion.identity);
        // 標的の座標
        Vector3 targetPosition = targetPoint;

        // 射出角度
        float angle = throwingAngle;

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


    public class Editor_AI_Poibot : Editor
    {
        public override void OnInspectorGUI()
        {

        }
    }
}
