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
    [SerializeField]
    private Text[] text = new Text[4];

    private void Start()
    {
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
                text[0].text = ("チーム1 得点" + point1);
                text[1].text = ("プレイヤー" + team1[0].playerID + "　ポイント" + team1[0].point +
                    "プレイヤー" + team1[1].playerID + "　ポイント" + team1[1].point);
                text[2].text = ("チーム2 得点" + point2);
                text[3].text = ("プレイヤー" + team2[0].playerID + "　ポイント" + team2[0].point +
                    "プレイヤー" + team2[1].playerID + "　ポイント" + team2[1].point);
            }
            else
            {
                text[0].text = ("チーム2 得点" + point2);
                text[1].text = ("プレイヤー" + team2[0].playerID + "　ポイント" + team2[0].point +
                    "プレイヤー" + team2[1].playerID + "　ポイント" + team2[1].point);
                text[2].text = ("チーム1 得点" + point1);
                text[3].text = ("プレイヤー" + team1[0].playerID + "　ポイント" + team1[0].point +
                    "プレイヤー" + team1[1].playerID + "　ポイント" + team1[1].point);
            }
        }
        else
        {
            var list = m_PlayerData.OrderByDescending(c => c.point).ToList();

            for (int i = 0; i < list.Count; i++)
                text[i].text = i + "位　プレイヤー" + list[i].playerID + "　ポイント" + list[i].point;

        }
    }
}

