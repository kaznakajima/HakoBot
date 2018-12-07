using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_TargetPos;
    private float speed = 5.0f;

    private void Start()
    {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            if (Vector3.Distance(transform.position, m_TargetPos) <= 0.05f)
            {
                transform.position = m_TargetPos;
                //爆発エフェクトを出す

                Observable.Timer(System.TimeSpan.FromSeconds(2.0f))
                .Subscribe(__ =>
                {
                    Destroy(gameObject);
                }).AddTo(this);
            }
            else
            {
                var dir = (m_TargetPos - transform.position).normalized;
                transform.Translate(dir * Time.deltaTime * speed);
            }
        }).AddTo(this);
    }
}
