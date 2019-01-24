using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_NewPoibot : MonoBehaviour
{
    [SerializeField]
    private PoibotData m_PoibotData;

    [SerializeField, Header("投てき物出現地点")]
    private Transform m_GenerationPosition;

    [SerializeField, Header("投てき位置")]
    private Vector3 m_ThrowPosition;
    [SerializeField, Header("補充位置")]
    private Vector3 m_PreparationPosition;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 1.0f;

    private bool m_Throwing;

    private void Start()
    {

    }

    private void PrepareForThrowing()
    {
        var list = m_PoibotData.m_Item.Where(c => c.m_Event);
        var number = Random.Range(0, list.Count());


    }

    private IEnumerator Move()
    {
        //移動目標地点を取得
        var pos = m_Throwing ? m_ThrowPosition : m_PreparationPosition;
        while (true)
        {
            //距離とベクトルを取得
            var dir = (pos - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, pos);

            //線形保管によって徐々に速度
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
        if (m_Throwing)
            m_Throwing = false;
        else
            PrepareForThrowing();
    }
}
