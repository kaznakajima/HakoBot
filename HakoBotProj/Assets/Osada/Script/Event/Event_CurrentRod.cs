using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace DigitalRuby.LightningBolt
{
    public class Event_CurrentRod : Event
    {
        //配置を行うためのロッドデータ
        [SerializeField]
        private CurrentRodData m_RodData;
        //設置を行うロッド
        [SerializeField]
        private GameObject m_RodPre;
        //設置したロッドの保存用List
        private List<GameObject> m_RodList = new List<GameObject>();
        //出現させたエフェクト保存用
        private List<GameObject> m_EffectList = new List<GameObject>();

        //電流エフェクト
        [SerializeField]
        private GameObject m_CurrentEffect;
        //ライン表示用オブジェクト
        [SerializeField]
        private GameObject m_CurrentLine;


        public override void EventStart()
        {
            //設置を行う
            PerformRodInstallation();
        }
        public override void EventEnd()
        {
            foreach (GameObject obj in m_EffectList)
                Destroy(obj);

            //指定された時間後ロッドも爆発させて破壊する
            Observable.Timer(System.TimeSpan.FromSeconds(1.0f)).
                Subscribe(_ =>
                {
                    foreach (GameObject rod in m_RodList)
                        rod.GetComponent<Rod>().Destroy();
                }).AddTo(this);
        }

        /// <summary>
        /// ロッドの設置を行う
        /// </summary>
        private void PerformRodInstallation()
        {
            //外側のロッドの設置
            foreach (Vector3 pos in m_RodData.m_OuterRodPos)
                m_RodList.Add(Instantiate(m_RodPre, new Vector3(pos.x, m_RodData.m_Height, pos.z), transform.rotation) as GameObject);

            //内側のロッドの設置位置を決めて、設置
            var count = 0;
            while (count < m_RodData.m_MaxRodNumber)
            {
                var range = m_RodData.m_Range;
                var x = Random.Range(m_RodData.m_CenterPos.x - range, m_RodData.m_CenterPos.x + range);
                var z = Random.Range(m_RodData.m_CenterPos.z - range, m_RodData.m_CenterPos.z + range);
                var pos = new Vector3(x, m_RodData.m_Height, z);
                //ここの距離は現在適当　後で修正予定
                var rodPosList = m_RodList.Select(c => c.transform.position).ToList();

                if (rodPosList.Any(c => Vector3.Distance(c, pos) < m_RodData.m_Distance))
                    continue;

                count++;
                m_RodList.Add(Instantiate(m_RodPre, pos, transform.rotation) as GameObject);
            }

            Observable.FromCoroutine(DropTheRod, publishEveryYield: false).
                Subscribe(_ => ActivateRod()).AddTo(this);
        }

        /// <summary>
        /// ロッドの有効化を行う
        /// </summary>
        private void ActivateRod()
        {
            //通過する順で有効化するロッドの番号を取得する
            var rodNumberList = DecideTheRodToActivate();
            var posList = new List<Vector3>();
            //指定されたロッドの電流化を有効化させ、座標をLineRendererに設定させる
            for (int i = 0; i < rodNumberList.Count(); i++)
            {
                var number = rodNumberList[i];
                m_RodList[number].GetComponent<Rod>().Activation();
                var pos = m_RodList[number].transform.position;
                pos.y += 1.6f;
                posList.Add(pos);
            }
            //LineRendererでどこに電流が流れる予定か表示する
            var lineObj = Instantiate(m_CurrentLine, posList[0], transform.rotation);
            m_EffectList.Add(lineObj);
            var lineRenderer = lineObj.GetComponent<LineRenderer>();
            lineRenderer.positionCount = posList.Count();
            for (int i = 0; i < posList.Count; i++)
            {
                lineRenderer.SetPosition(i, posList[i]);
            }

            //準備時間を設けてから電流を流す（予定）
            Observable.Timer(System.TimeSpan.FromSeconds(1.0f)).
                Subscribe(_ =>
                {
                    for(int i = 0; i < rodNumberList.Count() - 1; i++)
                    {
                        var startPos = m_RodList[rodNumberList[i]].transform;
                        var endPos = m_RodList[rodNumberList[i + 1]].transform;

                        var obj = Instantiate(m_CurrentEffect, startPos.position, transform.rotation) as GameObject;
                        m_EffectList.Add(obj);
                        LightningBoltScript[] bolt = obj.GetComponentsInChildren<LightningBoltScript>();
                        foreach (LightningBoltScript b in bolt)
                        {
                            b.StartObject = startPos.gameObject;
                            b.EndObject = endPos.gameObject;

                            b.StartPosition = new Vector3(0, 1.6f, 0);
                            b.EndPosition = new Vector3(0, 1.6f, 0);
                        }
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
            var startRodNumber = Random.Range(0, m_RodData.m_OuterRodPos.Count());
            routeRodNumber.Add(startRodNumber);

            //有効化する内側のロッドの本数を決める
            var number = Random.Range(1, m_RodData.m_MaxRodNumber + 1);
            var count = 0;
            while (count < number)
            {
                //有効化する内側のロッドの決定
                var rodNumber = Random.Range(m_RodData.m_OuterRodPos.Count(), m_RodList.Count());
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
                var endRodNumber = Random.Range(0, m_RodData.m_OuterRodPos.Count());
                //開始のロッドと被っていないか確認
                if (startRodNumber != endRodNumber)
                {
                    routeRodNumber.Add(endRodNumber);
                    break;
                }
            }
            return routeRodNumber.ToArray();
        }

        /// <summary>
        /// ロッドを落下させるためのコルーチン
        /// </summary>
        /// <param name="rodList">ロッド用List</param>
        /// <returns></returns>
        private IEnumerator DropTheRod()
        {
            while (true)
            {
                foreach (GameObject rod in m_RodList)
                {
                    var pos = new Vector3(rod.transform.position.x, 0, rod.transform.position.z);
                    var dis = Vector3.Distance(pos, rod.transform.position);
                    var vector = dis > 0.1f ? (pos - rod.transform.position).normalized : Vector3.zero;
                    rod.transform.position += vector * 5.0f * Time.deltaTime;
                }
                if(m_RodList.Select(c=>c.transform.position).All(c=>Vector3.Distance(c,new Vector3(c.x, 0, c.z)) < 0.1f))
                    break;

                yield return null;
            }
        }
    }
}
