using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Event_FallingObject : MonoBehaviour,Event
{
    [SerializeField]
    private GameObject objPre;

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
                var posX = Random.Range(-width, width);
                var posZ = Random.Range(-length, length);

                var obj = Instantiate(objPre, new Vector3(posX, 5.0f, posZ), transform.rotation) as GameObject;

                time.Value = Random.Range(1f, 3f);
            }).AddTo(disposables);
        }).AddTo(disposables);
    }

    public void EventEnd()
    {
        disposables.Dispose();
    }
}
