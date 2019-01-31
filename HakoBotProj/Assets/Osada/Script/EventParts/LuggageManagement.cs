using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LuggageManagement : MonoBehaviour
{
    private List<GameObject> luggageList = new List<GameObject>();

    public void ListAdd(GameObject obj)
    {
        luggageList.Add(obj);
    }
    public void RemoveList(GameObject obj)
    {
        luggageList.Remove(obj);
    }

    public int GetCount()
    {
        return luggageList.Count();
    }
}
