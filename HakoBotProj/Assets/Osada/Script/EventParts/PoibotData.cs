using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class PoiBotData : MonoBehaviour
{
    public enum ItemType
    {
        Baggage,
        HighBaggage,
    }
    //投てき物用構造体
    public struct Item
    {
        public GameObject items_object;
        public ItemType itemType;
        public bool valid;
    }
    public Item[] item = new Item[0];

    public float throwing_range_x;
    public float throwing_range_z;

#if UNITY_EDITOR
    [CustomEditor(typeof(PoiBotData))]
    public class Editor_CurrentRodData : Editor
    {
        bool folding = false;
        public override void OnInspectorGUI()
        {
            PoiBotData t = target as PoiBotData;

            int len = t.item.Length;
            len = EditorGUILayout.IntField("投てき物の個数", len);
            t.item = new Item[len];
            if (folding = EditorGUILayout.Foldout(folding, "投てき物リスト"))
            {
                // リスト表示
                for (int i = 0; i < len; ++i)
                {
                    EditorGUILayout.LabelField("投てき物 " + i + "  ゲームオブジェクト");
                    t.item[i].items_object = EditorGUILayout.ObjectField(t.item[i].items_object, typeof(GameObject), true) as GameObject;
                    t.item[i].itemType = (ItemType)EditorGUILayout.EnumPopup("投てき物のタイプ", t.item[i].itemType);
                    t.item[i].valid = EditorGUILayout.Toggle("投てき許可", t.item[i].valid);
                }
            }
            t.throwing_range_x = EditorGUILayout.FloatField("投てき範囲ｘ", t.throwing_range_x);
            t.throwing_range_z = EditorGUILayout.FloatField("投てき範囲z", t.throwing_range_z);
        }
    }
#endif
}
