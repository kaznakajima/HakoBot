using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;

/// <summary>
/// ゲームの全体を管理するクラス
/// </summary>
public class MainManager : SingletonMonobeBehaviour<MainManager>
{
    // プレイヤーの得点
    public int[] playerPoint = new int[4];

    // ゲームがスタートしているか
    [HideInInspector]
    public bool isStart;

    // ノイズ発生スクリプト参照
    CRT noise;
    // ノイズ用アニメーション
    Animator noiseAnim;
    // カウントダウン用アニメーション
    [SerializeField]
    GameObject countDown;

    // ゲーム終了処理
    [SerializeField]
    AnimationClip endClip;

    // 自身のAudioSource
    AudioSource myAudio;
    bool isPlay;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);

        myAudio = GetComponent<AudioSource>();

        noise = FindObjectOfType<CRT>();
        noiseAnim = noise.gameObject.GetComponent<Animator>();
        noiseAnim.SetTrigger("switchOff");

        if (SceneManager.GetActiveScene().name != "Prote")
        {     
            return;
        }

        // Character配置
        for(int i = 0;i < 4; i++)
        {
            // プレイヤーがエントリーしているならプレイヤー操作にする
            if (PlayerSystem.Instance.isActive[i])
            {
                // キャラクター用オブジェクトのインスタンス
                GameObject character = Instantiate(PlayerSystem.Instance.playerList[i]);

                character.AddComponent<Player>();
                character.GetComponent<Player>()._myNumber = i + 1;
            }
            // エントリーがされていないなら敵とする
            else
            {
                // キャラクター用オブジェクトのインスタンス
                GameObject character = Instantiate(PlayerSystem.Instance.enemyList[i]);

                // ランダムで敵AIのタイプを決める
                int enemyNum = UnityEngine.Random.Range(0, 3);
                switch (enemyNum)
                {
                    // 攻撃AI
                    case 0:
                        character.AddComponent<AttackEnemy>();
                        character.GetComponent<AttackEnemy>()._myNumber = i + 1;
                        break;
                    // バランスAI
                    case 1:
                        character.AddComponent<BalanceEnemy>();
                        character.GetComponent<BalanceEnemy>()._myNumber = i + 1;
                        break;
                    // 荷物優先AI
                    case 2:
                        character.AddComponent<TransportEnemy>();
                        character.GetComponent<TransportEnemy>()._myNumber = i + 1;
                        break;
                    default:
                        character.AddComponent<BalanceEnemy>();
                        character.GetComponent<BalanceEnemy>()._myNumber = i + 1;
                        break;
                }
            }
        }
	}

    // Update is called once per frame
    void Update () {
        if (SceneManager.GetActiveScene().name != "Prote")
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                noiseAnim.SetTrigger("switchOn");
                StartCoroutine(SceneNoise(2.0f, "Title"));
            }

            return;
        }

        CheckGameState();

        // ポーズ処理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isStart) {
                Pause();
            }
            else {
                Resume();
            }
        }
	}

    /// <summary>
    /// ポーズ処理
    /// </summary>
    void Pause()
    {
        Time.timeScale = 0.0f;
        isStart = false;
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    void Resume()
    {
        Time.timeScale = 1.0f;
        isStart = true;
    }

    // ゲームの状況を判断
    void CheckGameState()
    {
        if (isStart || isPlay)
            return;

        if (noise.Alpha == 0.0f) {
            isPlay = true;

            countDown.SetActive(true);

            var disposable = new SingleAssignmentDisposable();
            // 1秒ごとにカウント
            disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(time =>
            {
                AudioController.Instance.SEPlay("CountDown");
            }).AddTo(this);

            // 3秒後に移動再開
            Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
            {
                disposable.Dispose();
                AudioController.Instance.SEPlay("Start");
                isStart = true;
                isPlay = false;
            }).AddTo(this);
        }
    }

    // ゲーム終了
    public void GameEnd()
    {
        if (isPlay)
            return;

        Animation endAnim = countDown.GetComponent<Animation>();
        countDown.SetActive(true);
        endAnim.Play("CountDown_GameEnd");

        var disposable = new SingleAssignmentDisposable();
        // 1秒ごとにカウント
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(time =>
        {
            AudioController.Instance.SEPlay("CountDown");
        }).AddTo(this);

        // 3秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
        {
            disposable.Dispose();
            AudioController.Instance.SEPlay("End");
            isStart = false;
            noiseAnim.SetTrigger("switchOn");
            StartCoroutine(SceneNoise(2.0f, "Result"));
        }).AddTo(this);

        isPlay = true;
    }

    // シーン変更
    public IEnumerator SceneNoise(float _interval, string sceneName)
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
