using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light : MonoBehaviour {
    public Light spotlight;
    public GameObject rbot;
    Vector3 vector = Vector3.zero;
    float speed = 0.5f;
	// Use this for initialization
	void Start () {
       
      
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 spotlitpostion = spotlight.transform.position;
        Vector3 rbotpotion = rbot.transform.position;
        Vector3 mokutekipostion= new Vector3(rbotpotion.x, spotlitpostion.y, rbotpotion.z);
        spotlight.transform.position = Vector3.SmoothDamp(spotlitpostion, mokutekipostion, ref vector, speed);
        //spotlight.transform.position = new Vector3(rbotpotion.x, spotlitpostion.y, rbotpotion.z);

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
