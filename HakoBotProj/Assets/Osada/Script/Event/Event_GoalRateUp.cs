using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class Event_GoalRateUp : MonoBehaviour,Event
{
    private Goal[] goal = new Goal[4];

    public void EventStart()
    {
        var t = Random.Range(0, goal.Length);
        goal[t].rate = 2;
    }

    public void EventEnd()
    {
        foreach (Goal g in goal)
            g.rate = 1;
    }
}
