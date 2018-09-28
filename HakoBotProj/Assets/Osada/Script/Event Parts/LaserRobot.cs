using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LaserRobot : MonoBehaviour
{
    private void Start()
    {
        this.UpdateAsObservable().Subscribe(c =>
        {

        }).AddTo(this);
    }
}
