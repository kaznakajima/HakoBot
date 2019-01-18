using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DeliveryRobot : MonoBehaviour
{
    private GameObject itemPre;
    private Vector3 launchPos;

    private FloatReactiveProperty time;
    private void Start()
    {
        time.Value = Random.Range(2f, 5f);

        time.Subscribe(c =>
        {
            Observable.Timer(System.TimeSpan.FromSeconds(c))
            .Subscribe(x =>
            {
                var obj = Instantiate(itemPre, launchPos, transform.rotation) as GameObject;
                var rigidbody = obj.GetComponent<Rigidbody>();
                rigidbody.AddForce(obj.transform.up * 5.0f);

                time.Value = Random.Range(2f, 5f);
            }).AddTo(this);
        }).AddTo(this);
    }
}
