using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class image : MonoBehaviour
{
    

    [SerializeField]
    float alpha = 0.0f;
    float speed = 0.01f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        

        this.GetComponent<SpriteRenderer>().color=new Color(1.0f,1.0f,1.0f,alpha);
        //if(alpha <= 1.0f)
        //{
        //    alpha = alpha + speed;
        //}
        if (alpha > 0)
        {
            alpha = alpha - speed;
        }
        if (alpha <= 0.0f)
        {
            alpha = 1.0f;
        }

    }

    void Colorchenge()
    {    
        
    }
}
