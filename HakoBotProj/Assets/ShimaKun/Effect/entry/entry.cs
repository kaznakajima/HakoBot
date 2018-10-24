using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entry : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //ゲームパッド別で押したボタンでエントリーさせる
        // CubeプレハブをGameObject型で取得
        GameObject obj1p = (GameObject)Resources.Load("1p");
        GameObject obj2p = (GameObject)Resources.Load("2p");
        GameObject obj3p = (GameObject)Resources.Load("3p");
        GameObject obj4p = (GameObject)Resources.Load("4p");
        //1p
        if (Input.GetButtonDown("entry1"))//現状button1になっているので後々変更予定
        //if(Input.GetKey("1"))
        {
            Debug.Log("1p");
            //プレハブを元に、インスタンスを生成、
            Instantiate(obj1p, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        }
        //2p
        if (Input.GetButtonDown("entry2"))
        //if (Input.GetKey("2"))
        {
            Debug.Log("2p");
            Instantiate(obj2p, new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
        }
        //3p
        //if (Input.GetButtonDown("entry3"))
        if (Input.GetKey("3"))
        {
            Debug.Log("3p");
            Instantiate(obj3p, new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
        }
        //4p
        //if (Input.GetButtonDown("entry4"))
        if (Input.GetKey("4"))
        {
            Debug.Log("4p");
            Instantiate(obj4p, new Vector3(0.0f, 6.0f, 0.0f), Quaternion.identity);
        }

        //？？秒後にゲームを始めるかのログを出して、誰かがキーを入力したら開始を作る予定
    }
}
