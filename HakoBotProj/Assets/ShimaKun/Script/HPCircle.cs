using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCircle : MonoBehaviour
{
    GameObject iamge;

    [SerializeField]
    float a;
    // Use this for initialization
    void Start()
    {
        iamge = GameObject.Find("HPCircle");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void HPDown(float current, int max)
    {
        //float maxfill = 0.75f;

        iamge.GetComponent<Image>().fillAmount += current;

        
    }
}