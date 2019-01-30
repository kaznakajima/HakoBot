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
    [SerializeField]
    private EventMessageData m_MessageData;

    private BoolReactiveProperty m_Action = new BoolReactiveProperty(false);

    private float m_EventTime = 20f;
    private float m_EventInterval = 20f;

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

                  if (m_MessageData != null)
                      SendMessage(m_MessageData.message[number].m_EventMessage);

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

    private void SendMessage(string message)
    {
        //メッセージ表示用Scriptを取得
        //文字データを送信
    }
}
