using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reza : MonoBehaviour {

    
    private bool hitFlag = false;
    public Ray2D shotRay;
    private float distance = 10.0f;
    LineRenderer lineRenderer;
    public GameObject rezapot;
    private RaycastHit2D shotHit;
    private Vector2 nearPoint;
    // Use this for initialization
    public void Start () {
    
        lineRenderer = GetComponent<LineRenderer>();
       
        }
	
	// Update is called once per frame
	void Update () {
     
       

    }
    public void Shot()
    {
       
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rezapot.transform.position);
        shotRay.origin = rezapot.transform.position;
        shotRay.direction =transform.up * -1;
       
       
        Vector3 kz = shotRay.origin + shotRay.direction * distance;
        lineRenderer.SetPosition(0, rezapot.transform.position);
        lineRenderer.SetPosition(1, kz);
        float st = Vector2.Distance(rezapot.transform.position, kz);
        shotHit = Physics2D.Raycast(rezapot.transform.position, kz, st);
        
        if (shotHit.collider)
        {
            Debug.Log(hitFlag);
            nearPoint = shotHit.point;
            hitFlag = true;
           
            if (shotHit.collider.tag == "Wall")
            {
                Debug.Log("壁に当たった");
            }
            if (shotHit.collider.tag == "Player")
            {
                Debug.Log("Player");
            }
        }
        else
        {
            hitFlag=false;
        }
        if (hitFlag)
        {
            lineRenderer.SetPosition(1, nearPoint);
        }
        else
        {
            lineRenderer.SetPosition(1, kz);
        }
      

        

    }
    public void DisableEffect()
    {
        lineRenderer.enabled = false;
    }




}

