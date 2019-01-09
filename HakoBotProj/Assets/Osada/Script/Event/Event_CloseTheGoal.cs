using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_CloseTheGoal : Event
{
    [SerializeField]
    private PointArea[] m_PointArea = new PointArea[4];

    public override void EventStart()
    {
        var rand = Random.Range(0, m_PointArea.Length);
        m_PointArea[rand].Close();
    }

    public override void EventEnd()
    {
        foreach (PointArea _pointArea in m_PointArea)
        {
            if (_pointArea.isActive == false)
            {
                _pointArea.Open();
            }
        }
    }
}
