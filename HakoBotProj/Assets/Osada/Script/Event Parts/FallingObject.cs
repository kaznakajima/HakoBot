using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FallingObject : MonoBehaviour
{
    private void Start()
    {
        this.UpdateAsObservable().Subscribe(c =>
        {
            var pos = transform.position;
            pos.y -= 3.0f * Time.deltaTime;
            transform.position = pos;
        }).AddTo(this);
    }
}
