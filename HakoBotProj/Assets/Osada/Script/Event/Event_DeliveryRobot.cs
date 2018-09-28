using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_DeliveryRobot : MonoBehaviour,Event
{
    private GameObject robotPre;
    private GameObject robot;


    public void EventStart()
    {
        robot = Instantiate(robotPre, new Vector3(0, 0, 0), transform.rotation);
    }

    public void EventEnd()
    {
        Destroy(robot);
    }
}
