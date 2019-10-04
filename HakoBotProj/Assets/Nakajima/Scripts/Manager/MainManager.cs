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
    [HideInInspector]
    public int[] playerPoint = new int[4];

    // ゲームがスタートしているか
    [HideInInspector]
    public bool isStart;

    // ポーズ画面表示
    [SerializeField]
    private GameObject pauseImage;

    // ノイズ発生スクリプト参照
    private CRT noise;
    // ノイズ用アニメーション
    private Animator noiseAnim;
    // カウントダウン用アニメーション
    [SerializeField]
    private GameObject countDown;

    // ゲーム終了処理
    [SerializeField]
    private AnimationClip endClip;

    // 音楽が再生されているか
    private bool isPlay;

    // シーン上のAudioSourceのリスト
    private List<AudioSource> playingAudioSource = new List<AudioSource>();

    // プレイヤーデータ
    public List<PlayerData> playerData = new List<PlayerData>();

    // AIロボットの停止命令
    public bool isStop;

	// 初回処理
	void Start () {
        // ノイズフェード
        GetNoiseBase();
        noiseAnim.SetTrigger("switchOff");

        // Character配置
        for (int i = 0; i < 4; i++)
        {
            GameObject character;

            // プレイヤーがエントリーしているならプレイヤー操作にする
            if (PlayerSystem.Instance.isActive_GamePad[i])
            {
                // キャラクター用オブジェクトのインスタンス
                character = Instantiate(PlayerSystem.Instance.playerList[i]);

                // プレイヤークラスの追加
                character.AddComponent<Player>();

                // プレイヤ－番号の定義
                var player = character.GetComponent<Player>();
                player.myNumber = i + 1;

                // 自身のインプットステートを格納
                player.myInputState = player.myNumber;
            }
            // エントリーがされていないなら敵とする
            else
            {
                // キャラクター用オブジェクトのインスタンス
                character = Instantiate(PlayerSystem.Instance.enemyList[i]);

                character.AddComponent<BalanceEnemy>();
                character.GetComponent<BalanceEnemy>().myNumber = i + 1;
            }
        }
	}

    // 更新処理
    void Update () {
        GameStart();

        // ポーズ処理
        for(int i = 0;i < 4; i++) {
            if(PlayerSystem.Instance.Button_Pause(i + 1))
            {
                // ポーズ解除
                if (Time.timeScale != 0.0f) {
                    AudioController.Instance.SEPlay("Pause");
                    Pause();
                }
                // ポーズ実行
                else {
                    Resume();
                }
            }
        }
       
    }

    /// <summary>
    /// ノイズ発生用キャンバスの設定
    /// </summary>
    void GetNoiseBase()
    {
        // ノイズを発生させているクラスの取得
        foreach(CRT _noise in FindObjectsOfType<CRT>())
        {
            // シーン変更用に設定
            if(_noise.gameObject.name == "Noise") {
                noise = _noise;
                noiseAnim = noise.gameObject.GetComponent<Animator>();
            }
        }
    }

    /// <summary>
    /// ポーズ画面からタイトルへ
    /// </summary>
    public void PauseToTitle()
    {
        Resume();
        isStop = true;
        noiseAnim.SetTrigger("switchOn");
        StartCoroutine(SceneNoise(2.0f, "Title"));
    }

    /// <summary>
    /// ポーズ処理
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0.0f;

        pauseImage.SetActive(true);

        // 再生中なら一時停止
        foreach(var audio in GetAudioSource())
        {
            if (audio.isPlaying && audio != AudioController.Instance.myAudio[1]) {
                audio.Pause();
                playingAudioSource.Add(audio);
            }
        }
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;

        pauseImage.SetActive(false);

        // 再開
        foreach (var audio in playingAudioSource)
        {
            audio.UnPause();
        }
        playingAudioSource.Clear();
    }

    /// <summary>
    /// AudioSourceの取得
    /// </summary>
    /// <returns></returns>
    public AudioSource[] GetAudioSource()
    {
        return FindObjectsOfType<AudioSource>();
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    void GameStart()
    {
        // ゲーム開始時、ポーズ時はリターン
        if (isStart || isPlay) return;

        // ノイズがかかっていない状態に実行
        if (noise.Alpha == 0.0f) {
            isPlay = true;

            countDown.SetActive(true);

            var disposable = new SingleAssignmentDisposable();
            // 1秒ごとにカウント
            disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(time =>
            {
                AudioController.Instance.SEPlay("CountDown");
            }).AddTo(this);

            // 3秒後にゲームスタート
            Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
            {
                disposable.Dispose();
                AudioController.Instance.SEPlay("Start");
                isStart = true;
                isPlay = false;
            }).AddTo(this);
        }
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameEnd()
    {
        if (isPlay) return;

        Animation endAnim = countDown.GetComponent<Animation>();
        countDown.SetActive(true);
        endAnim.Play("CountDown_GameEnd");

        var disposable = new SingleAssignmentDisposable();
        // 1秒ごとにカウント
        disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(time =>
        {
            AudioController.Instance.SEPlay("CountDown");
        }).AddTo(this);

        // 3秒後に終了
        Observable.Timer(TimeSpan.FromSeconds(4.0f)).Subscribe(time =>
        {
            disposable.Dispose();
            AudioController.Instance.SEPlay("End");
            isStart = false;
            isStop = true;
            noiseAnim.SetTrigger("switchOn");
            StartCoroutine(SceneNoise(2.0f, "Result"));
        }).AddTo(this);

        isPlay = true;
    }

    /// <summary>
    /// シーン変更
    /// </summary>
    /// <param name="_interval">シーン変更までにかかる時間</param>
    /// <param name="sceneName">次のシーン名</param>
    /// <returns></returns>
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
