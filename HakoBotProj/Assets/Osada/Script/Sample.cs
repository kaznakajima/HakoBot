using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

//ポイボットAI改良
public class Sample : MonoBehaviour
{
    public enum ItemType
    {
        
    }
    //投てき物用構造体
    private struct Item
    {
        public GameObject m_ItemObj;
        public ItemType m_ItemType;
        public bool m_Event;
    }
    [SerializeField]
    private Item[] m_Item;
    [SerializeField]
    private Transform m_GenerationPosition;
    [SerializeField]
    private GameObject m_Marker;

    [SerializeField]
    private Vector3 m_ThrowPosition;
    [SerializeField]
    private Vector3 m_PreparationPosition;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 1.0f;

    private bool m_Throwing;

    private void Start()
    {
        PrepareForThrowing();
    }

    private void PrepareForThrowing()
    {
        m_Throwing = true;
        StartCoroutine(Move(m_ThrowPosition));
    }

    private void Throwing()
    {
        var objList = m_Item.Where(c => c.m_Event).ToList();
        var objNumber = Random.Range(0, objList.Count());

        switch (m_Item[objNumber].m_ItemType)
        {

        }

        m_Throwing = false;
        StartCoroutine(Move(m_PreparationPosition));
    }

    private IEnumerator Move(Vector3 position)
    {
        while (true)
        {
            var dir = (position - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, position);
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
            Throwing();
        else
            PrepareForThrowing();
    }
}
