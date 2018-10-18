using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEntry : MonoBehaviour
{

    // 操作するキャラクターのリスト
    [SerializeField]
    List<GameObject> playerList;

    // プレイヤーがエントリーしたかどうか
    public bool[] playerActive;

    // Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        EntrySystem();
	}

    /// <summary>
    /// プレイヤーのエントリー機能
    /// </summary>
    void EntrySystem()
    {
        for(int i = 0;i < 4; i++)
        {
            if (PlayerSystem.Instance.Button_A(i + 1))
            {
                Debug.Log("player" + i + 1 + "参加");
                PlayerSystem.Instance.isActive[i] = true;
                foreach(bool active in playerActive)
                {
                    Debug.Log(active);
                }
            }
            if (PlayerSystem.Instance.ButtonUp_A(i + 1))
            {
                SceneManager.LoadScene("Prote");
            }
        }
    }
}
