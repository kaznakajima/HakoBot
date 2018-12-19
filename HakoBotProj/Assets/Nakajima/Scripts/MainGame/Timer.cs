using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : SingletonMonobeBehaviour<Timer> {
    //制限時間
    private float totalTime;
    //分
    public float minute;
    //秒
    public float seconds;
    //前回のUpdate時の秒数
    private float oldseconds;
    //タイマー表示用のText
    public Text TimerText;
	// Use this for initialization
	void Start () {
        //制限時間の設定
        totalTime = minute * 60 + seconds;
        //oldsecondsの初期化
        oldseconds = 0f;

	}
	
	// Update is called once per frame
	void Update () {

        if (MainManager.Instance.isStart == false)
            return;

        //一旦制限時間を計測する
        totalTime = minute * 60 + seconds;
        //制限時間を減らす
        totalTime -= Time.deltaTime;
        //分の設定
        minute = (int)totalTime / 60;
        //秒の設定
        seconds = totalTime - minute * 60;
        //タイマー表示用Textに時間を表示する
        if ((int)seconds != (int)oldseconds)
        {
            TimerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
        }

        //制限時間が0になったら、EndGameメソッドを呼ぶ
        if (totalTime <= 4.0f)
        {
            EndGame();
            return;
        }
        //oldsecondsを設定する
        oldseconds = seconds;
	}
    //制限時間が0になったら呼ばれる
    void EndGame()
    {
        //ここに処理を書き込む
        MainManager.Instance.GameEnd();
    }
}
