using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Efect : MonoBehaviour {
     float efecttime;
    public float EfectTime;
    public float EfectTime2;
    public Color efectcolor;
    public Color efectcolor2;
    // Use this for initialization
    void Start () {
        efecttime = 0;
	}
	
	// Update is called once per frame
	void Update () {
        efecttime += Time.deltaTime;
        if (efecttime>=EfectTime)
        {
            ParticleSystem.MainModule par = GetComponent<ParticleSystem>().main;
            par.startColor =efectcolor;
            
        }
        if (efecttime>=EfectTime2)
        {
            ParticleSystem.MainModule par = GetComponent<ParticleSystem>().main;
            par.startColor =efectcolor2;
        }
    }
}
