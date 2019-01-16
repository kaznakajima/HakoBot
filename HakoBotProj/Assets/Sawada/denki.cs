using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class denki : MonoBehaviour {

    // Use this for initialization

    [SerializeField]
    Transform[] posList;

    [SerializeField]
    GameObject obj;
    private void Start()
    {
        foreach (Transform pos in posList)
        {
            obj.transform.position = pos.position;
        }
    }
}
