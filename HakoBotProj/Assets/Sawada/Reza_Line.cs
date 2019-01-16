using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reza_Line : MonoBehaviour {
    public Transform cube1;
    public Transform cube2;
    public Transform cube3;
    // Use this for initialization
    void Start () {
       
	}

    // Update is called once per frame
    void Update() {
        LineRenderer render = GetComponent<LineRenderer>();
        render.positionCount = 4;
        render.SetPosition(0, new Vector3(4, 0, 0));
        render.SetPosition(1, cube1.position);
        render.SetPosition(2, cube2.position);
        render.SetPosition(3, cube3.position);
        if (Input.GetKeyDown(KeyCode.A))
        {
            render.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            render.enabled = false;
        }
    }
}
