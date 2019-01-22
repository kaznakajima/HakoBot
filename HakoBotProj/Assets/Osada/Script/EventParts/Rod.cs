using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Rod : MonoBehaviour
{


    private BoolReactiveProperty m_Activation = new BoolReactiveProperty(false);

    private void Start()
    {
        m_Activation.
            Subscribe(c =>
            {

            }).AddTo(this);
    }

    //破壊処理
    public void Destroy()
    {

    }
}
