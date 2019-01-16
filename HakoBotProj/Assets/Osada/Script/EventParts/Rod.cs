using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Rod : MonoBehaviour
{
    //ロッドの有効化判定
    public BoolReactiveProperty m_Activation = new BoolReactiveProperty(false);

    private void Start()
    {
        m_Activation.Where(c => c).
            Subscribe(c =>
            {
                if (m_Activation.Value)
                {
                    //呼びエフェクトを出して指定された時間後、電流エフェクトを発生させる
                    Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
                    .Subscribe(_ =>
                    {
                        //電流エフェクトを発生させる
                    }).AddTo(this);
                }
            }).AddTo(this);
    }

    //破壊処理
    public void Destroy()
    {
        //ロッドの有効か判定をfalseに
        m_Activation.Value = false;
        Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
            .Subscribe(_ =>
            {
                //爆発エフェクト後、破壊
            }).AddTo(this);
    }
}
