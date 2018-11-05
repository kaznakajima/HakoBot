using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class FieldObjectManagement : MonoBehaviour
{
    List<Vector3> objectList = new List<Vector3>();

    private float posY = 1.0f;

    public void SetObjectPoint(Vector3 pos)
    {
        objectList.Add(pos);
    }

    public void RemoveObjectPoint(Vector3 pos)
    {
        objectList.Remove(pos);
    }

    public List<Vector3> GetObjectPoint()
    {
        var list = objectList.Where(c => c.y <= posY).ToList();
        return list;
    }
}
