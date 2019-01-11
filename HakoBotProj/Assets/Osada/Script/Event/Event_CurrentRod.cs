using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_CurrentRod : Event
{
    [SerializeField]
    private AI_Poibot m_Poibot;

    [SerializeField]
    private Vector3 m_CenterPos;
    [SerializeField]
    private float m_Width, m_Depth;

    private List<Vector3> m_ThrowPosList = new List<Vector3>();

    public override void EventStart()
    {

    }

    public override void EventEnd()
    {

    }

    //投てき位置検索
    public void ThrowPositionSearch(int n)
    {

    }

    //投てき範囲List作成
    public void CreateThrowingRange()
    {
        
    }
}
