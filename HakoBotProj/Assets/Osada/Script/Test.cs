using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Test : MonoBehaviour,Event
{
    private struct ObjectData
    {
        public GameObject obj;
        public float sizeX;
        public float sizeZ;
    }

    private ObjectData[] objectData;

    private float width = 10f;
    private float length = 10f;

    private FloatReactiveProperty time = new FloatReactiveProperty();

    private CompositeDisposable disposables = new CompositeDisposable();

    public void EventStart()
    {
        time.Value = Random.Range(1f, 3f);

        time.Subscribe(c =>
        {
            Observable.Timer(System.TimeSpan.FromSeconds(c))
            .Subscribe(x =>
            {
                //ここ編集箇所
                var posX = Random.Range(-width, width);
                var posZ = Random.Range(-length, length);

                var number = Random.Range(0, objectData.Length);
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
