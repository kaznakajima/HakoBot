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

    // 連続入力防止
    bool isDown;

    [SerializeField]
    CRT noise;

    // TimeLine
    public TitleSystem title;

    // Use this for initialization
    void Start () {
        title.m_TiteTimeline.Play();
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
        for (int i = 0; i < 4; i++)
        {
            if (PlayerSystem.Instance.Button_B(i + 1) && title.m_TiteTimeline.time != 0.0f)
            {
                title.m_TiteTimeline.time = 12.5f;
                title.m_StartTimeline.Play();
            }
        }

        if (title.m_StartTimeline.time < 4.0f)
            return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioController.Instance.SEPlay("Select");
            noise.myAnim.SetTrigger("switchOn");
            StartCoroutine(SceneNoise(2.0f));
        }

        for (int i = 0;i < 4; i++)
        {
            // エントリーさせる
            if (PlayerSystem.Instance.Button_B(i + 1) && isEntry[i] == false)
            {
                // コントローラーの振動
                VibrationController.Instance.PlayVibration(i, true);

                // ゲームパッドの番号のプレイヤーをアクティブにする
                PlayerSystem.Instance.isActive[i] = true;
                playerEntryList[i].SetBool("isEntry", true);
                AudioController.Instance.SEPlay("Entry");

                isEntry[i] = true;
            }
            // エントリー解除
            if (PlayerSystem.Instance.Button_A(i + 1) && isEntry[i])
            {
                // ゲームパッドの番号のプレイヤーを非アクティブにする
                PlayerSystem.Instance.isActive[i] = false;
                playerEntryList[i].SetBool("isEntry", false);
                AudioController.Instance.SEPlay("Cancel");

                isEntry[i] = false;
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
