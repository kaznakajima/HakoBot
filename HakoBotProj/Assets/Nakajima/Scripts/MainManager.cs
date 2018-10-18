using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの全体を管理するクラス
/// </summary>
public class MainManager : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        for(int i = 0;i < 4; i++)
        {
            if (PlayerSystem.Instance.isActive[i])
            {
                PlayerSystem.Instance.playerList[i].GetComponent<Player>().enabled = true;
                
            }
            Instantiate(PlayerSystem.Instance.playerList[i]);
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
