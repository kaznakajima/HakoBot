﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class Event_GoalToClose : Event
{
    //[SerializeField]
    //private Goal[] goal = new Goal[4];
    [SerializeField]
    private PointArea[] pointArea = new PointArea[4];

    public override void EventStart()
    {
        //var t = Random.Range(0, goal.Length);
        //goal[t].open = false;
        var rand = Random.Range(0, pointArea.Length);
        pointArea[rand].Close();
    }

    public override void EventEnd()
    {
        //foreach (Goal g in goal)
        //    g.open = true;
        foreach (PointArea _pointArea in pointArea)
        {
            if(_pointArea.isActive == false)
            {
                _pointArea.Open();
            }
        }
            
    }
}