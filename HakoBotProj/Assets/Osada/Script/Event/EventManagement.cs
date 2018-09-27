using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class EventManagement : MonoBehaviour
{
    [SerializeField]
    private List<Event> eventList = new List<Event>();

    private BoolReactiveProperty play = new BoolReactiveProperty(false);

    private float eventTime = 30f;
    private float eventInterval = 30f;

    private int target;

    private void Start()
    {
        //イベント開始
        play.Where(c => !c).Subscribe(c =>
          {
              Observable.Timer(System.TimeSpan.FromSeconds(eventInterval))
              .Subscribe(x =>
              {
                  target = Random.Range(0, eventList.Count);
                  //events[target].EventStart();
                  Debug.Log("イベント" + target + "スタート");
                  play.Value = true;
              }).AddTo(this);
          }).AddTo(this);

        //イベント終了
        play.Where(c => c).Subscribe(c =>
          {
              Observable.Timer(System.TimeSpan.FromSeconds(eventTime))
              .Subscribe(x =>
              {
                  //events[target].EventEnd();
                  Debug.Log("イベント" + target + "終了");
                  play.Value = false;
              }).AddTo(this);
          }).AddTo(this);
    }

    //[CustomEditor(typeof(EventManagement))]
    //public class Editor_EventManagement : Editor
    //{
    //    bool open = false;
    //    public override void OnInspectorGUI()
    //    {
    //        EventManagement eventManagement = target as EventManagement;

    //        eventManagement.eventTime = EditorGUILayout.FloatField("イベント発生時間", eventManagement.eventTime);
    //        eventManagement.eventInterval = EditorGUILayout.FloatField("インターバル時間", eventManagement.eventInterval);
    //    }
    //}
}
