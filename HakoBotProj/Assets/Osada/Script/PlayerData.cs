using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public int playerID;

    public int point;

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
}
