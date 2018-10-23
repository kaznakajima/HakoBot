using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

//妨害AI
//必要機能
//ターゲット決定　移動　攻撃
public class Test : MonoBehaviour
{
    private Transform[] player = new Transform[4];
    private Transform target;

    private NavMeshAgent nav;

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var dis = Vector3.Distance(transform.position, target.position);
            }).AddTo(this);
    }

    //ターゲット決定　ポイント差によって狙われる確率を変更させる予定
    private void TargetSearch()
    {
        var number = Random.Range(0, player.Length);
        target = player[number];
        Move();
    }

    //ターゲット付近まで移動
    private void Move()
    {
        nav.SetDestination(target.position);
    }

    //ターゲットに対して攻撃を実行
    private void Attack()
    {

    }
}
