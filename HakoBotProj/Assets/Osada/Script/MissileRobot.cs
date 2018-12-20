using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MissileRobot : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Missile;
    [SerializeField]
    private Transform m_LaunchPosition;

    //投てき位置
    [SerializeField]
    private Vector3 m_ThrowPosition;
    //移動開始位置
    [SerializeField]
    private Vector3 m_MoveStartPosition;

    private BoolReactiveProperty m_Moved = new BoolReactiveProperty(false);
    private Vector3 m_targetPos;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 1.0f;

    private float m_SizeX = 15.0f, m_SizeZ = 9.0f;

    [SerializeField]
    private GameObject m_Marker;

    private void Start()
    {
        EventStart();

        m_Moved.Where(c => c)
            .Subscribe(c =>
            {
                StartCoroutine("ThrowMove");
            }).AddTo(this);

        m_Moved.Where(c => !c)
            .Subscribe(c =>
            {
                StopAllCoroutines();
                StartCoroutine("Return");
            }).AddTo(this);

    }

    private IEnumerator ThrowMove()
    {
        var obj = Instantiate(m_Missile, m_LaunchPosition.position, transform.rotation) as GameObject;
        obj.transform.parent = transform;
        var scr = obj.GetComponent<Missile>();
        Setting();

        var pos = m_ThrowPosition;
        while (true)
        {
            var dir = (pos - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, pos);
            var currentVelocity = m_Velocity;
            m_Velocity = dis > 0.3f ? dir * m_Speed : Vector3.zero;
            m_Velocity = Vector3.Lerp(currentVelocity, m_Velocity, Mathf.Min(Time.deltaTime + 5.0f, 1));


            Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, 360.0f * Time.deltaTime);
            transform.position += m_Velocity * Time.deltaTime;

            if (dis < 0.3f)
            {
                scr.Setting(m_targetPos);
                obj.transform.parent = null;

                StartCoroutine("Return");
                break;
            }
            yield return null;
        }
    }

    private IEnumerator Return()
    {
        var pos = m_MoveStartPosition;
        while (true)
        {
            var dir = (pos - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, pos);
            var currentVelocity = m_Velocity;
            m_Velocity = dis > 0.3f ? dir * m_Speed : Vector3.zero;
            m_Velocity = Vector3.Lerp(currentVelocity, m_Velocity, Mathf.Min(Time.deltaTime + 5.0f, 1));


            Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, 360.0f * Time.deltaTime);
            transform.position += m_Velocity * Time.deltaTime;

            if (dis < 0.3f)
            {
                if (m_Moved.Value)
                {
                    StartCoroutine("ThrowMove");
                }
                break;
            }
            yield return null;
        }
    }

    public void EventStart()
    {
        m_Moved.Value = true;
    }
    public void EventEnd()
    {
        m_Moved.Value = false;
    }


    public void Setting()
    {
        var x = Random.Range(-m_SizeX, m_SizeX);
        var z = Random.Range(-m_SizeZ, m_SizeZ);

        var targetPos = new Vector3(x, 0.2f, z);
        m_targetPos = targetPos;

        Instantiate(m_Marker, targetPos, m_Marker.transform.rotation);
    }
}
