using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestryTextTime : MonoBehaviour
{
    public float Textlifetime;

    // Use this for initialization
    void Start ()
    {
        Destroy(gameObject, Textlifetime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
