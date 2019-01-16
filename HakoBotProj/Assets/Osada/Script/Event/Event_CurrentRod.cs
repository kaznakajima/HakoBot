using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class Event_CurrentRod : Event
{
    //外側のロッドの位置
    [SerializeField]
    private Vector3[] m_OuterRodPos = new Vector3[4];
    //ステージの中心座標
    [SerializeField]
    private Vector3 m_CenterPos;
    //設置を行っていい中心からの範囲（半径）
    [SerializeField]
    private float m_Range = 5.0f;

    //設置を行うロッド
    [SerializeField]
    private GameObject m_RodPre;
    //内側に設置するロッドの本数
    private int m_MaxRodNumber = 3;

    //設置したロッドの保存用List
    private List<Rod> m_RodList = new List<Rod>();

    //ライン表示用オブジェクト
    [SerializeField]
    private GameObject m_LineObj;

    private void Start()
    {
        //設置を行う
        PerformRodInstallation();
    }

    public override void EventStart()
    {
        //設置を行う
        PerformRodInstallation();
    }
    public override void EventEnd()
    {
        foreach (Rod rod in m_RodList)
        {
            rod.Destroy();
        }
    }

    /// <summary>
    /// ロッドの設置を行う
    /// </summary>
    private void PerformRodInstallation()
    {
        //外側のロッドの設置
        foreach (Vector3 pos in m_OuterRodPos)
        {
            var RodObj = Instantiate(m_RodPre, pos, transform.rotation) as GameObject;
            m_RodList.Add(RodObj.GetComponent<Rod>());
        }
        //内側のロッドの設置位置を決めて、設置
        for (int i = 0; i < m_MaxRodNumber; ++i)
        {
            var x = Random.Range(m_CenterPos.x - m_Range, m_CenterPos.x + m_Range);
            var z = Random.Range(m_CenterPos.z - m_Range, m_CenterPos.z + m_Range);
            var pos = new Vector3(x, m_CenterPos.y, z);
            var RodObj = Instantiate(m_RodPre, pos, transform.rotation) as GameObject;
            m_RodList.Add(RodObj.GetComponent<Rod>());
        }
        //設置が終了したら指定された時間後有効化するロッドを決めて、有効化を行う
        Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
            .Subscribe(_ =>
            {
                //通過する順で有効化するロッドの番号を取得する
                var rodNumberList = DecideTheRodToActivate();
                var pos = new List<Vector3>();
                //指定されたロッドの電流化を有効化させ、座標をTrailRendererに設定させる
                for (int i = 0; i < rodNumberList.Count(); i++)
                {
                    var number = rodNumberList[i];
                    m_RodList[number].m_Activation.Value = true;
                    pos.Add(m_RodList[number].gameObject.transform.position);
                }
                var lineRenderer = m_LineObj.GetComponent<LineRenderer>();
                lineRenderer.positionCount = pos.Count();
                for (int i = 0; i < pos.Count; i++)
                {
                    lineRenderer.SetPosition(i, pos[i]);
                }
            }).AddTo(this);
    }

    /// <summary>
    /// 有効化するロッドを決定する
    /// </summary>
    /// <returns>有効化するロッドの配列番号（有効化する順番になっている）</returns>
    private int[] DecideTheRodToActivate()
    {
        //ルート用List
        List<int> routeRodNumber = new List<int>();
        //開始のロッドを決める
        var startRodNumber = Random.Range(0, m_OuterRodPos.Length);
        routeRodNumber.Add(startRodNumber);

        //有効化する内側のロッドの本数を決める
        var number = Random.Range(1, m_MaxRodNumber + 1);
        var count = 0;
        while (count < number)
        {
            //有効化する内側のロッドの決定
            var rodNumber = Random.Range(m_OuterRodPos.Length, m_RodList.Count());
            //もう選択されているロッドではなかった場合、Listに追加しカウントを増やす
            if (routeRodNumber.All(c => c != rodNumber))
            {
                routeRodNumber.Add(rodNumber);
                count += 1;
            }
        }

        //目標地点のロッドを決める
        while (true)
        {
            var endRodNumber = Random.Range(0, m_OuterRodPos.Length);
            //開始のロッドと被っていないか確認
            if (startRodNumber != endRodNumber)
            {
                routeRodNumber.Add(endRodNumber);
                break;
            }
        }
        return routeRodNumber.ToArray();
    }
}
