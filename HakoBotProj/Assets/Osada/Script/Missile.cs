using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Missile : MonoBehaviour
{
    private Vector3[] m_Route = new Vector3[2];
    private int m_Count = 0;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 6.0f;

    private BoolReactiveProperty m_Moved = new BoolReactiveProperty(false);

    [SerializeField]
    private GameObject m_Explosion;

    private void Start()
    {
        m_Moved.Where(c => c)
            .Subscribe(c => StartCoroutine("Move")).AddTo(this);
    }

    private IEnumerator Move()
    {
        while (true)
        {
            var dir = (m_Route[m_Count] - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, m_Route[m_Count]);

            Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, 360.0f * Time.deltaTime);

            transform.position += transform.forward * m_Speed * Time.deltaTime;

            if (dis < 0.3f)
                m_Count += 1;

            if (m_Count < m_Route.Length)
                yield return null;
            else
            {
                m_Explosion.SetActive(true);
                Observable.Timer(System.TimeSpan.FromSeconds(0.5f)).Subscribe(__ => Destroy(gameObject)).AddTo(this);
                m_Moved.Value = false;
                break;
            }
        }
    }

    public void Setting(Vector3 targetPos)
    {
        var x = (transform.position.x - targetPos.x) / 2;
        var z = (transform.position.z - targetPos.z) / 2;

        m_Route[0] = new Vector3(x, transform.position.y + 3, z);
        m_Route[1] = targetPos;

        m_Moved.Value = true;
    }
}
