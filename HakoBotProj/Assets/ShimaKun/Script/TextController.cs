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

        scoreText[0].text = "1位" + PlayerCheck(1) + playerData[0].point;
        scoreText[1].text = "2位" + PlayerCheck(2) + playerData[1].point;
        scoreText[2].text = "3位" + PlayerCheck(3) + playerData[2].point;
        scoreText[3].text = "4位" + PlayerCheck(4) + playerData[3].point;
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

