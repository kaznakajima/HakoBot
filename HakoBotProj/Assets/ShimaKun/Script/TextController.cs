using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextController : SingletonMonobeBehaviour<TextController>
{
    [SerializeField]
    private List<PlayerData> playerData = new List<PlayerData>();
    public Text[] scoreText = new Text[4];

    // Use this for initialization
    void Start () {
        RankingStart();

        scoreText[0].text = "1位" + playerData[0].point + "ポイント" + PlayerCheck(1).ToString();
        scoreText[1].text = "2位" + playerData[1].point + "ポイント" + PlayerCheck(2).ToString();
        scoreText[2].text = "3位" + playerData[2].point + "ポイント" + PlayerCheck(3).ToString();
        scoreText[3].text = "4位" + playerData[3].point + "ポイント" + PlayerCheck(4).ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Rank()
        
    {
        
    }

    public void RankingStart()
    {
        var list = playerData.OrderByDescending(c => c.point).ToList();
        playerData = list;
    }

    public int PlayerCheck(int rank)
    {
        return playerData[rank - 1].playerID;
    }
}

