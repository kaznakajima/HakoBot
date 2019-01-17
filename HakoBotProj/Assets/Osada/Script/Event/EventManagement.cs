using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class EventManagement : MonoBehaviour
{
    [SerializeField]
    private List<Event> m_eventList = new List<Event>();

    private BoolReactiveProperty m_Action = new BoolReactiveProperty(false);

    private float m_EventTime = 30f;
    private float m_EventInterval = 30f;

    private void Start()
    {
        var number = 0;
        //イベント開始
        m_Action.Where(c => !c).Subscribe(c =>
          {
              Observable.Timer(System.TimeSpan.FromSeconds(m_EventInterval))
              .Subscribe(x =>
              {
                  number = Random.Range(0, m_eventList.Count);
                  m_eventList[number].EventStart();
                  Debug.Log("イベント" + number + "スタート");
                  m_Action.Value = true;
              }).AddTo(this);
          }).AddTo(this);

        //イベント終了
        m_Action.Where(c => c).Subscribe(c =>
          {
              Observable.Timer(System.TimeSpan.FromSeconds(m_EventTime))
              .Subscribe(x =>
              {
                  m_eventList[number].EventEnd();
                  Debug.Log("イベント" + number + "終了");
                  m_Action.Value = false;
              }).AddTo(this);
          }).AddTo(this);
    }
}
