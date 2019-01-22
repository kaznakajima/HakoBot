using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Rod : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_Explosion;
    [SerializeField]
    private ParticleSystem m_Current;

    public void Activation()
    {
        m_Current.Play();
    }

    //破壊処理
    public void Destroy()
    {
        m_Explosion.Play();
        Observable.Timer(System.TimeSpan.FromSeconds(1.0f)).
            Subscribe(_ =>
            {
                Destroy(gameObject);
            }).AddTo(this);
    }
}
