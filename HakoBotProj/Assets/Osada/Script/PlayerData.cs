using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public int playerID;

    public int point;

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerData))]
    public class Editor_PlayerData : Editor
    {
        public override void OnInspectorGUI()
        {
            PlayerData p = target as PlayerData;

            p.playerID = EditorGUILayout.IntField("プレイヤーID", p.playerID);
            p.point = EditorGUILayout.IntField("ポイント", p.point);
        }
    }
#endif
}
