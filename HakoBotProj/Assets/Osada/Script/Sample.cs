using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private List<PlayerData> m_PlayerData = new List<PlayerData>();

    public enum Type
    {
        Main,
        Sub
    }

    [System.Serializable]
    public class PanelData
    {
        public GameObject m_Panel;
        public Text[] m_Text;
        public Type m_Type;
    }
    [SerializeField]
    private PanelData[] m_PanelData = new PanelData[2];

    private void Start()
    {
        foreach(PanelData panel in m_PanelData)
        {
            if (panel.m_Type == Type.Main)
            {
                panel.m_Panel.SetActive(true);
                if (m_PlayerData.Count(c => c.m_Team == PlayerData.Team.Team1) == 2)
                {
                    var team1 = m_PlayerData.Where(c => c.m_Team == PlayerData.Team.Team1).ToList();
                    var team2 = m_PlayerData.Where(c => c.m_Team == PlayerData.Team.Team2).ToList();

                    var point1 = 0;
                    var point2 = 0;
                    foreach (PlayerData team in team1)
                        point1 += team.point;
                    foreach (PlayerData team in team2)
                        point2 += team.point;

                    if (point1 >= point2)
                    {
                        panel.m_Text[0].text = ("チーム1 得点" + point1);
                        panel.m_Text[1].text = ("　ポイント" + team1[0].point + "　ポイント" + team1[1].point);
                        panel.m_Text[2].text = ("チーム2 得点" + point2);
                        panel.m_Text[3].text = ("　ポイント" + team2[0].point + "　ポイント" + team2[1].point);
                    }
                    else
                    {
                        panel.m_Text[0].text = ("チーム2 得点" + point2);
                        panel.m_Text[1].text = ("プレイヤー" + team2[0].playerID + "　ポイント" + team2[0].point +
                            "プレイヤー" + team2[1].playerID + "　ポイント" + team2[1].point);
                        panel.m_Text[2].text = ("チーム1 得点" + point1);
                        panel.m_Text[3].text = ("プレイヤー" + team1[0].playerID + "　ポイント" + team1[0].point +
                            "プレイヤー" + team1[1].playerID + "　ポイント" + team1[1].point);
                    }
                }
                else
                {
                    var list = m_PlayerData.OrderByDescending(c => c.point).ToList();

                    for (int i = 0; i < list.Count; i++)
                        panel.m_Text[i].text = (i + 1) + "位　プレイヤー" + list[i].playerID + "　ポイント" + list[i].point;

                }
            }
            else
                panel.m_Panel.SetActive(false);
        }
    }
}

