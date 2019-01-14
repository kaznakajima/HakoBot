using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_CurrentRod : Event
{
    private List<Rod> m_RodList = new List<Rod>();

    [SerializeField]
    private GameObject m_RodPre;

    [SerializeField]
    private List<Vector3> m_CenterPosList = new List<Vector3>();
    [SerializeField]
    private float m_Range;

    public override void EventStart()
    {
        //ココ適当後で治す
        foreach(Vector3 centerPos in m_CenterPosList)
        {
            var list = Search(centerPos, m_Range);
            foreach(Vector3 pos in list)
            {
                var obj = Instantiate(m_RodPre, pos, transform.rotation) as GameObject;
                m_RodList.Add(obj.GetComponent<Rod>());
            }
        }
    }

    public override void EventEnd()
    {
        foreach(Rod rod in m_RodList)
        {
            rod.Destroy();
        }
    }

    public List<Vector3> Search(Vector3 CenterPos, float Range)
    {
        //ココ適当後で治す
        List<Vector3> posList = new List<Vector3>();
        for (int i = 0; i < 3; i++)  
        {
            var x = Random.Range(CenterPos.x - Range, CenterPos.x + Range);
            var z = Random.Range(CenterPos.z - Range, CenterPos.z + Range);
            posList.Add(new Vector3(x, 1, z));
        }
        return posList;
    }
}
