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
        vertical,
        Horizontal
    }
    public Direction direction;

    //移動範囲
    private float moveRange;

    private float speed;

    //荷物プレハブ
    private GameObject baggagePre;

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(c =>
            {

            }).AddTo(this);


        var time = Random.Range(3, 5);
        //ランダムの時間で荷物を投げさせる
        Observable.Timer(System.TimeSpan.FromSeconds(time)).
            Subscribe(c =>
            {

                time = Random.Range(3, 5);
            }).AddTo(this);
    }

    private void Move()
    {
        //前進
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void Turn()
    {
        //反転した時の角度
        var dir = Quaternion.Euler(0, transform.rotation.y + 90, 0);
    }
}
