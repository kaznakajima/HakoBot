using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class CurrentRodData : ScriptableObject
{
    public List<Vector3> m_OuterRodPos = new List<Vector3>();
    public Vector3 m_CenterPos;
    public float m_Range;
    public int m_MaxRodNumber;
    public float m_Distance;
    public float m_Height;

#if UNITY_EDITOR
    [CustomEditor(typeof(CurrentRodData))]
    public class Editor_CurrentRodData : Editor
    {
        bool folding = false;
        int number = 0;
        Vector3 pos = new Vector3(0, 0, 0);

        public override void OnInspectorGUI()
        {
            CurrentRodData d = target as CurrentRodData;

            List<Vector3> list = d.m_OuterRodPos;
            int i, len = list.Count;
            if (folding = EditorGUILayout.Foldout(folding, "外側に設置するロッドの座標"))
            {
                // リスト表示
                for (i = 0; i < len; ++i)
                {
                    list[i] = EditorGUILayout.Vector3Field("追加された座標" + i, list[i]);
                }

                Vector3 AddPos = EditorGUILayout.Vector3Field("追加する座標", pos);
                pos = AddPos;
                if (GUILayout.Button("座標追加"))
                    list.Add(AddPos);
                number = EditorGUILayout.IntSlider("削除したいList番号", number, 0, list.Count - 1);
                if (GUILayout.Button("指定された番号の配列を削除"))
                    list.Remove(list[number]);
            }

            d.m_CenterPos = EditorGUILayout.Vector3Field("ステージの中心座標", d.m_CenterPos);
            d.m_Range = EditorGUILayout.FloatField("ステージの半径", d.m_Range);
            d.m_MaxRodNumber = EditorGUILayout.IntSlider("内側のロッドの本数", d.m_MaxRodNumber, 1, 5);
            d.m_Distance = EditorGUILayout.FloatField("ロッド間の距離", d.m_Distance);
            d.m_Height = EditorGUILayout.FloatField("ロッドの出現高さ", d.m_Height);

            if (GUILayout.Button("保存"))
            {
                //ダーティとしてマークする(変更があった事を記録する)
                EditorUtility.SetDirty(d);

                //保存する
                AssetDatabase.SaveAssets();
            }
        }
    }
#endif
}
