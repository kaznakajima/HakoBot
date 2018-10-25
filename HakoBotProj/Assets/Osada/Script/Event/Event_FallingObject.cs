using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Event_FallingObject : MonoBehaviour,Event
{
    [SerializeField]
    private struct ObjectData
    {
        public GameObject obj;
        public float sizeX;
        public float sizeZ;
    }
    [SerializeField]
    private ObjectData[] objectData;

    //ステージの縦横の半径
    private float width = 10f;
    private float length = 10f;
    //荷物を投げるまでの時間
    private FloatReactiveProperty time = new FloatReactiveProperty();

    private CompositeDisposable disposables = new CompositeDisposable();

    public void EventStart()
    {
        time.Value = Random.Range(1f, 3f);

        time.Subscribe(c =>
        {
            Observable.Timer(System.TimeSpan.FromSeconds(c))
            .Subscribe(_ =>
            {
                var number = Random.Range(0, objectData.Length);

                //ここ編集箇所
                var posX = Random.Range(-width + objectData[number].sizeX, width - objectData[number].sizeX);
                var posZ = Random.Range(-length + objectData[number].sizeZ, length - objectData[number].sizeZ);

                Instantiate(objectData[number].obj, new Vector3(posX, 5.0f, posZ), transform.rotation);

                time.Value = Random.Range(1f, 3f);
            }).AddTo(disposables);
        }).AddTo(disposables);
    }

    public void EventEnd()
    {
        disposables.Dispose();
    }
}
