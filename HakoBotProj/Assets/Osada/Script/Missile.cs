using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_TargetPos;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 6.0f;

    [SerializeField]
    private GameObject m_ExplosionObj;

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var dir = (m_TargetPos - transform.position).normalized;
                var dis = Vector3.Distance(transform.position, m_TargetPos);
                var currentVelocity = m_Velocity;
                m_Velocity = dis > 0.3f ? dir * m_Speed : Vector3.zero;
                m_Velocity = Vector3.Lerp(currentVelocity, m_Velocity, Mathf.Min(Time.deltaTime + 5.0f, 1));


                Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, 360.0f * Time.deltaTime);
                transform.position += m_Velocity * Time.deltaTime;

                if (dis < 0.3f)
                {
                    m_ExplosionObj.SetActive(true);

                    Observable.Timer(System.TimeSpan.FromSeconds(0.5f)).Subscribe(__ => Destroy(gameObject)).AddTo(this);
                }
            }).AddTo(this);
    }
}
