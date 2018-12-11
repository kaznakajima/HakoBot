﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Sample : MonoBehaviour
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
    private float m_Speed = 6.0f;

    private void Start()
    {
        m_Moved.Where(c => c)
            .Subscribe(c =>
            {
                StartCoroutine("Move", m_ThrowPosition);
            }).AddTo(this);
    }

    private IEnumerator ThrowMove(Vector3 pos)
    {
        var obj = Instantiate(m_Missile, m_LaunchPosition.position, transform.rotation) as GameObject;
        obj.transform.parent = transform;
        var scr = obj.GetComponent<Missile>();

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

                pos = m_MoveStartPosition;
                StartCoroutine("Return", pos);
                break;
            }
            yield return null; 
        }
    }

    private IEnumerator Return(Vector3 pos)
    {
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
                break;

            yield return null;
        }
    }

    public void Setting(Vector3 targetPos)
    {
        if (!m_Moved.Value)
        {
            m_Moved.Value = true;
            m_targetPos = targetPos;
        }
    }
}
