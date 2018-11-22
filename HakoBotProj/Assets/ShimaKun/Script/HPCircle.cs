using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCircle : MonoBehaviour
{
    GameObject iamge;
	// Use this for initialization
	void Start () {
        iamge = GameObject.Find("HPCircle");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void HPDown(float current,int max)
    {
        iamge.GetComponent<Image>().fillAmount = current / max;
        
    }
}
