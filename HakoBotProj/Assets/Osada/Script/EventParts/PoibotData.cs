using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoibotData : ScriptableObject
{
    public enum ItemType
    {
        Baggage,
        HighBaggage,
        Missile,
        Keibot
    }
    //投てき物用構造体
    public struct Item
    {
        [Header("投てき物")]
        public GameObject m_ItemObj;
        [Header("アイテムタイプ")]
        public ItemType m_ItemType;
        [Header("投げていいものかの判断（触るの禁止）")]
        public bool m_Event;
    }
    public Item[] m_Item;

    [SerializeField, Header("横の半径")]
    private float m_SizeX;
    [SerializeField, Header("奥行の半径")]
    private float m_SizeZ;
}
