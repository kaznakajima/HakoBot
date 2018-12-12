using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Sample : Event
{
    private Ranking ranking;
    private Text[] text = new Text[4];
    private void Start()
    {
        text[0].text = "1位　プレイヤー" + ranking.PlayerCheck(1);
        text[1].text = "2位　プレイヤー" + ranking.PlayerCheck(2);
        text[2].text = "3位　プレイヤー" + ranking.PlayerCheck(3);
        text[3].text = "4位　プレイヤー" + ranking.PlayerCheck(4);
    }
}
