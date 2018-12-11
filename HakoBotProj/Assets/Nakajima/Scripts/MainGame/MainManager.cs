using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームの全体を管理するクラス
/// </summary>
public class MainManager : SingletonMonobeBehaviour<MainManager>
{
    // プレイヤーの得点
    public int[] playerPoint = new int[4];

    // ノイズ用アニメーション
    Animator noiseAnim;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);

        noiseAnim = FindObjectOfType<CRT>().gameObject.GetComponent<Animator>();
        noiseAnim.SetTrigger("switchOff");

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
                int enemyNum = Random.Range(0, 3);
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
		
	}
}
