using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Rod : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] m_Explosion;
    [SerializeField]
    private ParticleSystem[] m_Current;

    public void Activation()
    {
        foreach (ParticleSystem s in m_Current)
            s.Play();
    }

    //破壊処理
    public void Destroy()
    {
        foreach (ParticleSystem s in m_Explosion)
            s.Play();
        Observable.Timer(System.TimeSpan.FromSeconds(0.8f)).
            Subscribe(_ =>
            {
                Destroy(gameObject);
            }).AddTo(this);
    }
}
