using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// エントリー機能(仮実装)
/// </summary>
public class PlayerEntry : MonoBehaviour
{
    // エントリー用アニメーション
    [SerializeField]
    Animator[] playerEntryList;

    // プレイヤーが存在するか
    bool[] isEntry = new bool[4];

    bool isChange;

    [SerializeField]
    CRT noise;

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
        if (Input.GetKeyDown(KeyCode.Z)) {
            AudioController.Instance.SEPlay("Select");
            noise.myAnim.SetTrigger("switchOn");
            StartCoroutine(SceneNoise(2.0f));
        }



        for(int i = 0;i < 4; i++)
        {
            if (PlayerSystem.Instance.Button_B(i + 1))
            {
                Debug.Log("player" + i + 1 + "参加");
                PlayerSystem.Instance.isActive[i] = true;
                playerEntryList[i].SetBool("isEntry", true);
                isEntry[i] = true;
            }
            if (PlayerSystem.Instance.Button_A(i + 1) && isEntry[i])
            {
                playerEntryList[i].SetBool("isEntry", false);
                PlayerSystem.Instance.isActive[i] = false;
            }
        }
    }

    // シーン変更
    public IEnumerator SceneNoise(float _interval)
    {
        float time = 0.0f;
        while (time <= _interval)
        {
            time += Time.deltaTime;
            yield return null;
        }

        AudioController.Instance.BGMChange("Main");
        SceneManager.LoadScene("Prote");
    }
}
