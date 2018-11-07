using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

public class AI_Keibot : MonoBehaviour
{
    [SerializeField]
    private Transform[] player = new Transform[4];
    
    private Transform target;
    [SerializeField]
    private NavMeshAgent nav;

    private void Start()
    {
        TargetSearch();

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (target != null)
                {
                    var dis = Vector3.Distance(transform.position, target.position);
                    //指定距離まで近づけば
                    if (dis > 5f) Attack();
                }
            }).AddTo(this);
    }

    //ターゲット決定　ポイント差によって狙われる確率を変更させる予定
    private void TargetSearch()
    {
        //各プレイヤーのポイントと合計ポイントを計算
        var point = new int[] { 0, 0, 0, 0 };
        var pointAll = 0;
        foreach (int c in point) pointAll += c;
        //ターゲットナンバーを決定(１～１００)
        var number = Random.Range(1, 101);
        //各プレイヤーのナンバーを決定
        var rank1 = Mathf.RoundToInt(100 * (point[0] / pointAll));
        var rank2 = Mathf.RoundToInt(100 * (point[1] / pointAll)) + rank1;
        var rank3 = Mathf.RoundToInt(100 * (point[2] / pointAll)) + rank2;
        var rank4 = Mathf.RoundToInt(100 * (point[3] / pointAll)) + rank3;
        //ターゲットの決定
        if (number <= rank1) target = player[0];
        else if (number <= rank2) target = player[1];
        else if (number <= rank3) target = player[2];
        else target = player[3];

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
        //移動を停止
        nav.SetDestination(transform.position);
        //攻撃処理
        var chr = GetComponent(typeof(Character)) as Character;
        chr.Attack();
        //ターゲット再検索
        Observable.Timer(System.TimeSpan.FromSeconds(2.0f))
            .Subscribe(_ => TargetSearch());
    }
}
