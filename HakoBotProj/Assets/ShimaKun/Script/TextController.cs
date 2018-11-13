using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : SingletonMonobeBehaviour<TextController>
{
    public Text scoreText1;
    public Text scoreText2;
    public Text scoreText3;
    public Text scoreText4;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Rank(
        int playernumber1, int point1,
        int playernumber2, int point2, 
        int playernumber3, int point3,
        int playernumber4, int point4)
    {
        Debug.Log("らんく");

        scoreText1.text = playernumber1 + "P" + point1.ToString() + "点";
        scoreText2.text = playernumber2 + "P" + point2.ToString() + "点";
        scoreText3.text = playernumber3 + "P" + point3.ToString() + "点";
        scoreText4.text = playernumber4 + "P" + point4.ToString() + "点";
    }
}

