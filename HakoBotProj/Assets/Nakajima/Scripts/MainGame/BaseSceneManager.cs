using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManager : MonoBehaviour
{
    // ノイズ発生スクリプト参照
    CRT noise;
    // ノイズ用アニメーション
    [HideInInspector]
    public Animator noiseAnim;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// ノイズ発生用キャンバスの設定
    /// </summary>
    public virtual void GetNoiseBase()
    {
        foreach (CRT _noise in FindObjectsOfType<CRT>())
        {
            if (_noise.gameObject.name == "Noise")
            {
                noise = _noise;
                noiseAnim = noise.gameObject.GetComponent<Animator>();
            }
        }
    }

    /// <summary>
    /// シーン遷移ノイズ発生
    /// </summary>
    /// <param name="_interval">ノイズをかける時間</param>
    /// <param name="sceneName">次のシーン名</param>
    /// <returns></returns>
    public virtual IEnumerator SceneNoise(float _interval, string sceneName)
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
