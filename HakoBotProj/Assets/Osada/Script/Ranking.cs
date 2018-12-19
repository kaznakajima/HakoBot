using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class Ranking : MonoBehaviour
{
    [SerializeField]
    private List<PlayerData> playerData = new List<PlayerData>();

    public void RankingStart()
    {
        var list = playerData.OrderByDescending(c => c).ToList();
        playerData = list;
    }

    public int PlayerCheck(int rank)
    {
        return playerData[rank - 1].playerID;
    }
}
