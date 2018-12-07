using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private GameObject effectObj;
    [SerializeField]
    private Vector3 m_TargetPos;
    private float speed = 1.0f;

    private void Start()
    {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            if (Vector3.Distance(transform.position, m_TargetPos) <= 0.4f)
            {
                transform.position = m_TargetPos;
                //爆発エフェクトを出す
                effectObj.SetActive(true);

                Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                .Subscribe(__ =>
                {
                    Destroy(gameObject);
                }).AddTo(this);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, m_TargetPos, speed * Time.deltaTime);
                transform.localRotation = Quaternion.LookRotation((m_TargetPos - transform.position),Vector3.up);
            }
        }).AddTo(this);
    }
}
