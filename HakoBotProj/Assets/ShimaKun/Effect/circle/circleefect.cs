using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleefect : MonoBehaviour
{
    [SerializeField]
    private float interval = 5f;
    [SerializeField]
    private float CountTime = 0;
    public float mintime = 1f;
    public float x = 1f;
    public float y = 1f;
    public float z = 1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CountTime += Time.deltaTime;
        this.transform.localScale -= new Vector3(x * mintime, y * mintime, z);
        if (CountTime >= interval)
        {
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            this.transform.localScale -= new Vector3(x * mintime, y * mintime, z);
            CountTime = 0;
        }
    }
    
}
