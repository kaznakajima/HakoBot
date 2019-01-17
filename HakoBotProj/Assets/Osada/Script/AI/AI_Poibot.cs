using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class AI_Poibot : MonoBehaviour
{
    public enum ItemType
    {
        Baggage,
        HighBaggage,
        Missile,
        Keibot,
        Rod
    }
    //投てき物用構造体
    [System.Serializable]
    public struct Item
    {
        [Header("投てき物")]
        public GameObject m_ItemObj;
        [Header("アイテムタイプ")]
        public ItemType m_ItemType;
        [Header("投げていいものかの判断（触るの禁止）")]
        public bool m_Event;
    }
    [SerializeField,Header("投てき物リスト")]
    public Item[] m_Item;
    [SerializeField,Header("投てき物出現地点")]
    private Transform m_GenerationPosition;
    [SerializeField,Header("投てき位置マークオブジェクト")]
    private GameObject m_Marker;

    [SerializeField,Header("投てき位置")]
    private Vector3 m_ThrowPosition;
    [SerializeField, Header("補充位置")]
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
            case ItemType.Baggage:
            case ItemType.HighBaggage:
                break;
            case ItemType.Missile:
                break;
            case ItemType.Keibot:
                break;
            case ItemType.Rod:
                break;
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
