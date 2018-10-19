using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poibot_Move : MonoBehaviour
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
    [SerializeField]
    private float range;
    //移動開始位置と目標位置
    private Vector3 startPos, endPos;
    //移動開始時間保存用
    private float startTime = 0.0f;

    /// <summary>
    /// 移動処理の準備
    /// </summary>
    public void MoveSetUp(System.Action action)
    {
        //開始位置と目標位置を設定
        startPos = transform.position;
        endPos = SearchMoveTarget();
        //開始時間を保存
        startTime = Time.timeSinceLevelLoad;
        //移動処理を開始
        StartCoroutine("Move",action);
    }

    /// <summary>
    /// 移動目標地点の決定
    /// </summary>
    /// <returns>移動目標地点</returns>
    private Vector3 SearchMoveTarget()
    {
        var t = Random.Range(-range, range);
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
    /// 移動処理の実行
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move(System.Action action)
    {
        //移動にかかる時間を設定
        var time = Random.Range(3.0f, 5.0f);
        while (true)
        {
            //現在の経過時間
            var diff = Time.timeSinceLevelLoad - startTime;
            //割合計算
            var rate = diff / time;

            if (rate <= 1)
                //移動処理
                transform.position = Vector3.Lerp(startPos, endPos, rate);
            else
                break;

            yield return null;
        }
        action();
    }
}
