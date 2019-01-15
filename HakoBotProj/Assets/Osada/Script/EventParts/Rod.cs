using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Rod : MonoBehaviour
{
    //ロッドの有効化判定
    public bool m_Activation
    {
        set
        {
            m_Activation = value;
            if (m_Activation)
            {
                //呼びエフェクトを出して指定された時間後、電流エフェクトを発生させる
                Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
                    .Subscribe(_ =>
                    {
                        //電流エフェクトを発生させる
                    }).AddTo(this);
            }
        }
        get { return m_Activation; }
    }

    //破壊処理
    public void Destroy()
    {
        //ロッドの有効か判定をfalseに
        m_Activation = false;
        Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
            .Subscribe(_ =>
            {
                //爆発エフェクト後、破壊
            }).AddTo(this);
    }
}
