using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private Vector3 pos;

    private Vector3 dir = Vector3.zero;
    private Vector3 vec = Vector3.zero;
    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.rotation = Quaternion.LookRotation(pos - transform.position);

                vec = dir;
                dir = (pos - transform.position).normalized;
                dir = Vector3.Lerp(vec, dir, Time.deltaTime);
                Debug.Log(dir);

                transform.position = dir * Time.deltaTime;
            }).AddTo(this);
    }
}
