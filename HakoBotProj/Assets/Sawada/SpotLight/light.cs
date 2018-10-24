using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light : MonoBehaviour {
   
    public GameObject rbot;
    public Light spotlight;
	// Use this for initialization
	void Start () {

    
    }
	
	// Update is called once per frame
	void Update () {
        var aim =rbot.transform.position - spotlight.transform.position;
        var look = Quaternion.LookRotation(aim);
        spotlight.transform.localRotation = look;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (spotlight.enabled == true)
            {
                spotlight.enabled = false;
            }
           else if (spotlight.enabled == false)
            {
                spotlight.enabled = true;
            }
        }
	}
}
