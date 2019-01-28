using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Result : BaseSceneManager
{
    // リザルト用TimeLine
    [SerializeField]
    PlayableDirector tl_Result;

    // リザルト用テキスト
    [SerializeField]
    GameObject[] resultText;

    // TimeLineが再生されているか
    bool isPlay;

	// Use this for initialization
	void Start () {
        GetNoiseBase();
        isPlay = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (tl_Result.time < 7.0f)
            return;

        for (int i = 0; i < 4; i++)
        {
            if (PlayerSystem.Instance.Button_A(i + 1))
            {
                noiseAnim.SetTrigger("switchOn");
                StartCoroutine(SceneNoise(2.0f, "Title"));
            }
            if (PlayerSystem.Instance.Button_B(i + 1))
            {
                noiseAnim.SetTrigger("switchOn");
                StartCoroutine(SceneNoise(2.0f, "Main"));
            }
        }

        if (isPlay == false)
            return;

        ShowResultText();
    }

    /// <summary>
    /// リザルト用テキストの表示
    /// </summary>
    void ShowResultText()
    {
        foreach (GameObject obj in resultText)
            obj.SetActive(true);

        isPlay = false;
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
