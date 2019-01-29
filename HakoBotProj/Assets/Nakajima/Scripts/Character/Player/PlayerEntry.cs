using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// エントリー機能(仮実装)
/// </summary>
public class PlayerEntry : BaseSceneManager
{
    int pattern = 0;
    enum TEAM_PLAYER
    {
        PATTERN_1 = 0,
        PATTERN_2,
        PATTERN_3,
        PATTERN_4,
    }
    TEAM_PLAYER teamPattern;

    // それぞれのプレイヤーデータ
    [SerializeField]
    private List<PlayerData> playerData = new List<PlayerData>();

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

    // チーム戦表示
    [SerializeField]
    GameObject[] teamLogo;
    [SerializeField]
    Image[] numImage;
    [SerializeField]
    Sprite[] numSprite;

    int num = 0;

    // Use this for initialization
    void Start () {
        PlayerSystem.Instance.isTeam = false;
        GetNoiseBase();
        noiseAnim.SetTrigger("switchOff");
        title.m_TiteTimeline.Play();

        // 個人戦に切り替え
        pattern = 0;
        teamPattern = (TEAM_PLAYER)pattern;

        TeamBattle(teamPattern);
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

            // チーム戦切り替え
            if(PlayerSystem.Instance.Button_RightShoulder(i + 1) && PlayerSystem.Instance.isActive[i] == true)
            {
                if (PlayerSystem.Instance.isTeam == false) PlayerSystem.Instance.isTeam = true;

                // パターン変更
                pattern++;
                if (pattern > 3)
                    pattern = 1;

                foreach(GameObject obj in teamLogo) obj.SetActive(true);

                teamPattern = (TEAM_PLAYER)pattern;
                TeamBattle(teamPattern);
            }
            // 個人戦切り替え
            if (PlayerSystem.Instance.Button_LeftShoulder(i + 1) && PlayerSystem.Instance.isActive[i] == true)
            {
                if (PlayerSystem.Instance.isTeam == true) PlayerSystem.Instance.isTeam = false;

                // 個人戦に切り替え
                pattern = 0;
                teamPattern = (TEAM_PLAYER)pattern;

                foreach (GameObject obj in teamLogo) obj.SetActive(false);

                TeamBattle(teamPattern);
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
    /// チーム戦の切り替え
    /// </summary>
    /// <param name="team">チーム状態</param>
    void TeamBattle(TEAM_PLAYER team)
    {
        switch(team)
        {
            // 個人戦に切り替え
            case TEAM_PLAYER.PATTERN_1:
                for(int teamNum = 0; teamNum < 4; teamNum++) {
                    playerData[teamNum].m_Team = (PlayerData.Team)teamNum;
                }
                break;
            // パターン(1P 2Pペア, 3P 4Pペア)
            case TEAM_PLAYER.PATTERN_2:
                for (int teamNum = 0; teamNum < 2; teamNum++) {
                    playerData[teamNum].m_Team = 0;
                    numImage[teamNum].sprite = numSprite[0];
                    numImage[teamNum + 4].sprite = numSprite[0];
                }
                for (int teamNum = 2; teamNum < 4; teamNum++) {
                    playerData[teamNum].m_Team = (PlayerData.Team)1;
                    numImage[teamNum].sprite = numSprite[1];
                    numImage[teamNum + 4].sprite = numSprite[1];
                }
                break;
            // パターン(1P 3Pペア, 2P 4Pペア)
            case TEAM_PLAYER.PATTERN_3:
                for (int teamNum = 0; teamNum < 2; teamNum++) {
                    playerData[teamNum].m_Team = (PlayerData.Team)teamNum;
                    numImage[teamNum].sprite = numSprite[teamNum];
                    numImage[teamNum + 4].sprite = numSprite[teamNum];
                }
                for (int teamNum = 2; teamNum < 4; teamNum++) {
                    playerData[teamNum].m_Team = (PlayerData.Team)teamNum - 2;
                    numImage[teamNum].sprite = numSprite[teamNum - 2];
                    numImage[teamNum + 4].sprite = numSprite[teamNum - 2];
                }
                break;
            // パターン(1P 4Pペア, 2P 3Pペア)
            case TEAM_PLAYER.PATTERN_4:
                for (int teamNum = 0; teamNum < 2; teamNum++) {
                    playerData[teamNum].m_Team = (PlayerData.Team)teamNum;
                    numImage[teamNum].sprite = numSprite[teamNum];
                    numImage[teamNum + 3].sprite = numSprite[teamNum];
                }
                for (int teamNum = 2; teamNum < 4; teamNum++) {
                    playerData[teamNum].m_Team = (PlayerData.Team)Mathf.Abs(teamNum - 3);
                    numImage[teamNum].sprite = numSprite[Mathf.Abs(teamNum - 3)];
                    numImage[teamNum + 4].sprite = numSprite[Mathf.Abs(teamNum - 3)];
                }
                break;
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
