﻿using System.Collections;
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

	// Use this for initialization
	void Start () {

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
	}

    // ゲームの状況を判断
    void CheckGameState()
    {
        if (isStart)
            return;

        if (noise.Alpha == 0.0f) {
            countDown.SetActive(true);
            // 3秒後に移動再開
            Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
            {
                isStart = true;
            }).AddTo(this);
        }
    }

    // ゲーム終了
    public void GameEnd()
    {
        Animation endAnim = countDown.GetComponent<Animation>();
        countDown.SetActive(true);
        endAnim.Play("CountDown_GameEnd");
        // 3秒後に移動再開
        Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(time =>
        {
            isStart = false;
            noiseAnim.SetTrigger("switchOn");
            StartCoroutine(SceneNoise(2.0f, "Result"));
        }).AddTo(this);
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

        SceneManager.LoadScene(sceneName);
    }
}
