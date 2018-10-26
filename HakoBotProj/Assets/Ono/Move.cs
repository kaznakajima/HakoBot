using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float beamtimr;
    private GameObject player;
    private GameObject houdai;
    public float chaging;
    float chgingcont;
    public float rotaionSpeed;
    private float stpa = 1;
    private GameObject rezapot;
    float timer = 0.0f;

    // Use this for initialization
    void Start()
    {
        houdai = GameObject.Find("zyuusinn");
        player = GameObject.Find("player");
        rezapot = GameObject.Find("reza");
        chgingcont = 0;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forword = transform.up * -1;
        //自分が向いている方向
        Vector3 target = GameObject.FindGameObjectWithTag("Player").transform.position - this.transform.position;

        float myDot = Vector3.Dot(forword, target);

        float kyori = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position);

        if (myDot >= 2.0f && kyori <= 12.0f)
        {
            chgingcont += Time.deltaTime;
            // Debug.Log("みえる");
           // Debug.Log(chgingcont);

            Vector3 tardatdir = houdai.transform.position - player.transform.position;
            Quaternion toRotion = Quaternion.FromToRotation(Vector3.up, tardatdir);
            houdai.transform.rotation = Quaternion.Lerp(houdai.transform.rotation, toRotion, rotaionSpeed * stpa * Time.deltaTime);

         
        }
        else
        {
            if (timer==0)
            {
                chgingcont = 0;
            }
            else
            {
                chgingcont += Time.deltaTime;
            }
            
        }
        if (chgingcont > chaging)
        {
            stpa = 0;
            reza d = rezapot.GetComponent<reza>();
            d.Shot();
            timer += Time.deltaTime;
        }
        if (timer >= beamtimr)
        {
            timer = 0;
            reza d1= rezapot.GetComponent<reza>();
          d1.DisableEffect();
            stpa = 1;
            chgingcont = 0;
        }

    }
    
}
