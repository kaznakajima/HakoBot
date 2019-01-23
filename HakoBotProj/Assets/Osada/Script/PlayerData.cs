using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public enum Team
    {
        Team1,
        Team2,
        Team3,
        Team4
    }
    public Team m_Team;

    public int playerID;

    public int point;

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerData))]
    public class Editor_PlayerData : Editor
    {
        public override void OnInspectorGUI()
        {
            PlayerData p = target as PlayerData;

            p.m_Team = (Team)EditorGUILayout.EnumPopup("チーム番号", p.m_Team);

            p.playerID = EditorGUILayout.IntField("プレイヤーID", p.playerID);
            p.point = EditorGUILayout.IntField("ポイント", p.point);
        }
    }
#endif
}
