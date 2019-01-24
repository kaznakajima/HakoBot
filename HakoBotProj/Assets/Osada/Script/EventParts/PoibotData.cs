using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
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
    public Item[] m_Item = new Item[0];

    [SerializeField, Header("横の半径")]
    private float m_SizeX;
    [SerializeField, Header("奥行の半径")]
    private float m_SizeZ;

#if UNITY_EDITOR
    [CustomEditor(typeof(PoibotData))]
    public class Editor_CurrentRodData : Editor
    {
        bool folding = false;
        public override void OnInspectorGUI()
        {
            PoibotData d = target as PoibotData;

            int len = d.m_Item.Length;
            len = EditorGUILayout.IntField("投てき物の個数", len);
            d.m_Item = new Item[len];
            if (folding = EditorGUILayout.Foldout(folding, "投てき物リスト"))
            {
                // リスト表示
                for (int i = 0; i < len; ++i)
                {
                    EditorGUILayout.LabelField("投てき物 " + i + "  ゲームオブジェクト");
                    d.m_Item[i].m_ItemObj = EditorGUILayout.ObjectField(d.m_Item[i].m_ItemObj, typeof(GameObject), true) as GameObject;
                    d.m_Item[i].m_ItemType = (ItemType)EditorGUILayout.EnumPopup("投てき物のタイプ", d.m_Item[i].m_ItemType);
                    d.m_Item[i].m_Event = EditorGUILayout.Toggle("投てき許可", d.m_Item[i].m_Event);
                }                
            }
        }
    }
#endif
}
