using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// エントリー機能(仮実装)
/// </summary>
public class PlayerEntry : BaseSceneManager
{
    // エントリー用アニメーション
    [SerializeField]
    Animator[] playerEntryList;

    // プレイヤーが存在するか
    bool[] isEntry = new bool[4];

    // TimeLine
    public TitleSystem title;

    // タイトルテキスト
    [SerializeField]
    GameObject[] titleAnim;
    [SerializeField]
    GameObject startAnim;

    int num = 0;

    // Use this for initialization
    void Start () {
        GetNoiseBase();
        noiseAnim.SetTrigger("switchOff");
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
                titleAnim[0].SetActive(false);
                titleAnim[1].SetActive(false);
            }
        }

        if (title.m_StartTimeline.time < 4.0f)
            return;

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

                num--;
                if (num == 0) startAnim.SetActive(false);
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
                num++;
                if (num == 1)  startAnim.SetActive(true);
            }

            // Xボタンでゲームスタート
            if (PlayerSystem.Instance.Button_X(i + 1) && PlayerSystem.Instance.isActive[i] == true)
            {
                AudioController.Instance.SEPlay("Select");
                noiseAnim.SetTrigger("switchOn");
                StartCoroutine(SceneNoise(2.0f, "Main"));
            }
        }
    }
    
    /// <summary>
     /// ノイズ発生用キャンバスの設定
     /// </summary>
    public override void GetNoiseBase()
    {
        base.GetNoiseBase();
    }

    /// <summary>
    /// シーン遷移ノイズ発生
    /// </summary>
    /// <param name="_interval">ノイズをかける時間</param>
    /// <param name="sceneName">次のシーン名</param>
    /// <returns></returns>
    public override IEnumerator SceneNoise(float _interval, string sceneName)
    {
        return base.SceneNoise(_interval, sceneName);
    }
}
