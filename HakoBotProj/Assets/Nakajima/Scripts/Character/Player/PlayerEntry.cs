using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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

    // タイトルテキスト
    [SerializeField]
    GameObject titleAnim;

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
            if (PlayerSystem.Instance.Button_A(i + 1) && title.m_TiteTimeline.time != 0.0f)
            {
                title.m_TiteTimeline.time = 12.5f;
                title.m_StartTimeline.Play();
                titleAnim.SetActive(false);
            }
        }

        if (title.m_StartTimeline.time < 4.0f)
            return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioController.Instance.SEPlay("Select");
            noise.myAnim.SetTrigger("switchOn");
            StartCoroutine(SceneNoise("Main", 2.0f));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // コントローラーの振動
            VibrationController.Instance.PlayVibration(0, true);
            // ゲームパッドの番号のプレイヤーをアクティブにする
            PlayerSystem.Instance.isActive[0] = true;
            playerEntryList[0].SetBool("isEntry", true);
            AudioController.Instance.SEPlay("Entry");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // コントローラーの振動
            VibrationController.Instance.PlayVibration(1, true);
            // ゲームパッドの番号のプレイヤーをアクティブにする
            PlayerSystem.Instance.isActive[1] = true;
            playerEntryList[1].SetBool("isEntry", true);
            AudioController.Instance.SEPlay("Entry");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // コントローラーの振動
            VibrationController.Instance.PlayVibration(2, true);
            // ゲームパッドの番号のプレイヤーをアクティブにする
            PlayerSystem.Instance.isActive[2] = true;
            playerEntryList[2].SetBool("isEntry", true);
            AudioController.Instance.SEPlay("Entry");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // コントローラーの振動
            VibrationController.Instance.PlayVibration(3, true);

            // ゲームパッドの番号のプレイヤーをアクティブにする
            PlayerSystem.Instance.isActive[3] = true;
            playerEntryList[3].SetBool("isEntry", true);
            AudioController.Instance.SEPlay("Entry");
        }

        for (int i = 0;i < 4; i++)
        {
            // エントリー解除
            if (PlayerSystem.Instance.Button_B(i + 1) && isEntry[i])
            {
                // ゲームパッドの番号のプレイヤーを非アクティブにする
                PlayerSystem.Instance.isActive[i] = false;
                playerEntryList[i].SetBool("isEntry", false);
                AudioController.Instance.SEPlay("Cancel");

                isEntry[i] = false;
            }
            // エントリーさせる
            if (PlayerSystem.Instance.Button_A(i + 1) && isEntry[i] == false)
            {

                // コントローラーの振動
                VibrationController.Instance.PlayVibration(i, true);

                // ゲームパッドの番号のプレイヤーをアクティブにする
                PlayerSystem.Instance.isActive[i] = true;
                playerEntryList[i].SetBool("isEntry", true);
                AudioController.Instance.SEPlay("Entry");

                isEntry[i] = true;
            }
        }
    }

    /// <summary>
    ///  シーン変更
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <param name="_interval">インターバル</param>
    /// <returns></returns>
    public IEnumerator SceneNoise(string sceneName, float _interval)
    {
        float time = 0.0f;
        while (time <= _interval)
        {
            time += Time.deltaTime;
            yield return null;
        }

        AudioController.Instance.BGMChange(sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
