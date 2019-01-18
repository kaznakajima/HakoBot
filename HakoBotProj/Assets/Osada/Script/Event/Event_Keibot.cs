using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Event_Keibot : Event
{
    [SerializeField]
    private AI_Poibot[] m_Poibot;

    public override void EventStart()
    {
        foreach (AI_Poibot poibot in m_Poibot)
        {
            var item = poibot.m_Item.FirstOrDefault(c => c.m_ItemType == AI_Poibot.ItemType.Keibot);
            item.m_Event = true;
        }
    }
    public override void EventEnd()
    {
        foreach (AI_Poibot poibot in m_Poibot)
        {
            var item = poibot.m_Item.FirstOrDefault(c => c.m_ItemType == AI_Poibot.ItemType.Keibot);
            item.m_Event = false;
        }
    }
}
