using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Ranking : MonoBehaviour
{
    [SerializeField]
    private List<PlayerData> m_PlayerData = new List<PlayerData>();

    [SerializeField]
    private Text[] m_PlayerText = new Text[4];

    private void Start()
    {
        var list = m_PlayerData.OrderByDescending(c => c.point).ToList();
        ChangeColorOfText(list);

        m_PlayerText[0].text = "1位　プレイヤー" + list[0].playerID + "　ポイント" + list[0].point;
        m_PlayerText[1].text = "2位　プレイヤー" + list[1].playerID + "　ポイント" + list[1].point;
        m_PlayerText[2].text = "3位　プレイヤー" + list[2].playerID + "　ポイント" + list[2].point;
        m_PlayerText[3].text = "4位　プレイヤー" + list[3].playerID + "　ポイント" + list[3].point;
    }

    private void ChangeColorOfText(List<PlayerData> playerData)
    {
        for(int i = 0; i < m_PlayerText.Length; i++)
        {
            switch (playerData[i].playerID)
            {
                case 1:
                    m_PlayerText[i].color = Color.red;
                    break;
                case 2:
                    m_PlayerText[i].color = Color.blue;
                    break;
                case 3:
                    m_PlayerText[i].color = Color.yellow;
                    break;
                case 4:
                    m_PlayerText[i].color = Color.green;
                    break;
            }
        }
    }
}
