using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class EventMessageData : ScriptableObject 
{
    public struct Message
    {
        public string m_EventMessage;
    }
    public Message[] message = new Message[1];

#if UNITY_EDITOR
    [CustomEditor(typeof(EventMessageData))]
    public class Editor_EventMessageData : Editor
    {
        bool folding = false;

        public override void OnInspectorGUI()
        {
            EventMessageData t = target as EventMessageData;

            int len = t.message.Length;
            len = EditorGUILayout.IntField("イベントの個数", len);
            t.message = new Message[len];

            if (folding = EditorGUILayout.Foldout(folding, "イベントメッセージ"))
            {
                for(int i = 0; i < len; i++)
                {

                    EditorGUILayout.LabelField("イベント番号 " + i + " イベントメッセージ");
                    t.message[i].m_EventMessage = EditorGUILayout.TextArea(t.message[i].m_EventMessage, GUILayout.Height(80));
                }
            }
        }
    }
#endif
}
