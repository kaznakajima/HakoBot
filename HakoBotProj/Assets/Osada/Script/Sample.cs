using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

public class Sample : MonoBehaviour
{
    //外側のロッドの位置
    [SerializeField]
    private Vector3[] m_OuterRodPos = new Vector3[4];
    //ステージの中心座標
    [SerializeField]
    private Vector3 m_CenterPos;
    //設置を行っていい中心からの範囲（半径）
    [SerializeField]
    private float m_Range;
    //設置を行うロッド
    [SerializeField]
    private GameObject m_RodPre;
    //内側に設置するロッドの本数
    [SerializeField]
    private int m_MaxRodNumber;
    //設置したロッドの保存用List
    private List<Rod> m_RodList = new List<Rod>();


    public void EventStart()
    {
        //設置を行う
        PerformRodInstallation();
    }
    public void EventEnd()
    {
        foreach(Rod rod in m_RodList)
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
                //後で記述する予定あり
                for(int i = 0; i < rodNumberList.Count(); i++)
                {

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
        while (true)
        {
            //有効化する内側のロッドの決定
            var rodNumber = Random.Range(m_OuterRodPos.Length +1,m_RodList.Count());
            //もう選択されているロッドではなかった場合、Listに追加しカウントを増やす
            if (routeRodNumber.Any(c => c != rodNumber))
            {
                routeRodNumber.Add(rodNumber);
                count += 1;
                //指定の本数に到達したら終了
                if (count >= number)
                    break;
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

