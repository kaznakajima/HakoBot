using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Event_HighPointLuggage : Event
{
    [SerializeField]
    private Poibot_Throwing[] throwing;

    public override void EventStart()
    {
        foreach (Poibot_Throwing p in throwing)
            p.playEvent = true;
    }

    public override void EventEnd()
    {
        foreach (Poibot_Throwing p in throwing)
            p.playEvent = false;
    }
}
