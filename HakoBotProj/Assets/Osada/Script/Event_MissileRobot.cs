using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_MissileRobot : Event
{
    [SerializeField]
    private List<MissileRobot> m_Robot = new List<MissileRobot>();
    public override void EventStart()
    {
        foreach (MissileRobot robot in m_Robot)
        {
            robot.EventStart();
        }
    }

    public override void EventEnd()
    {
        foreach (MissileRobot robot in m_Robot)
        {
            robot.EventEnd();
        }
    }
}
