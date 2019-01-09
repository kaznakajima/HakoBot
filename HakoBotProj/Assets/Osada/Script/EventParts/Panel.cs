using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Panel : MonoBehaviour
{
    public BoolReactiveProperty m_Activation = new BoolReactiveProperty(false);

    private void Start()
    {
        m_Activation.Where(c => c)
            .Subscribe(c =>
            {

            }).AddTo(this);
    }

    public void OnCollisionEnter(Collision collision)
    {

    }
}
