using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Event_HighPointLuggage : MonoBehaviour,Event
{
    [SerializeField]
    private Poibot_Throwing[] throwing;

    public void EventStart()
    {
        foreach (Poibot_Throwing p in throwing)
            p.playEvent = true;
    }

    public void EventEnd()
    {
        foreach (Poibot_Throwing p in throwing)
            p.playEvent = false;
    }
}
