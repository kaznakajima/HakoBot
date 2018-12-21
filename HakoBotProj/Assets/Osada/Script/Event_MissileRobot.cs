using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_MissileRobot : Event
{
    [SerializeField]
    private List<MissileRobot> m_Robot = new List<MissileRobot>();

    // 横の壁のシャッター
    [SerializeField]
    List<SpaceArea> shutter = new List<SpaceArea>();

    public override void EventStart()
    {
        foreach (MissileRobot robot in m_Robot)
        {
            robot.EventStart();
        }

        // シャッター開く
        foreach (SpaceArea space in shutter)
        {
            space.Open();
        }
    }

    public override void EventEnd()
    {
        foreach (MissileRobot robot in m_Robot)
        {
            robot.EventEnd();
        }
        
        // シャッター閉じる
        foreach (SpaceArea space in shutter)
        {
            space.Close();
        }
    }
}
