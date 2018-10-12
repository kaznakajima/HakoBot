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
        Horizontal  //横移動
    }
    public Direction direction;
    //移動範囲
    private float moveRange;
    //移動速度
    private float speed;
    //移動中かの判断
    private BoolReactiveProperty actionEnd = new BoolReactiveProperty(false);

    //荷物プレハブ
    private GameObject baggagePre;

    //移動開始位置と目標位置
    private Vector3 startPos, endPos;
    //移動開始時間保存用
    private float startTime = 0.0f;

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

            }).AddTo(this);
    }

    //移動目標地点検索
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

    //移動処理
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
}
