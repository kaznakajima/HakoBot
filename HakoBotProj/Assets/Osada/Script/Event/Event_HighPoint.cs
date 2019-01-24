using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Event_HighPoint : Event
{
    [SerializeField]
    private AI_Poibot[] m_Poibot;

    public override void EventStart()
    {
        for(int i = 0; i < m_Poibot.Length; i++)
        {
            var item=m_Poibot[i].m_Item.FirstOrDefault(c => c.m_ItemType == AI_Poibot.ItemType.HighBaggage);
            var number = m_Poibot[i].m_Item.IndexOf(item);
            m_Poibot[i].m_Item[number].m_Event = true;
        }
    }
    public override void EventEnd()
    {
        for (int i = 0; i < m_Poibot.Length; i++)
        {
            var item = m_Poibot[i].m_Item.FirstOrDefault(c => c.m_ItemType == AI_Poibot.ItemType.HighBaggage);
            var number = m_Poibot[i].m_Item.IndexOf(item);
            m_Poibot[i].m_Item[number].m_Event = false;
        }
    }
}
