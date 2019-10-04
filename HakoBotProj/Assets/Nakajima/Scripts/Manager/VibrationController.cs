using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using XInputDotNetPure;

/// <summary>
/// コントローラー振動クラス
/// </summary>
public class VibrationController : SingletonMonobeBehaviour<VibrationController>
{
    protected override void Awake()
    {
        base.Awake();
    }

    // 初回処理
    void Start () {
        DontDestroyOnLoad(gameObject);
	}

    /// <summary>
    /// コントローラーのバイブレーション
    /// </summary>
    /// <param name="playerNum">プレイヤー番号</param>
    /// <param name="isVibration">バイブレーションをさせるかどうか</param>
    public void PlayVibration(int playerNum, bool isVibration)
    {
        // バイブレーションの強度を決める
        float value = isVibration ? 1.0f : 0.0f;
        PlayerIndex index = (PlayerIndex)playerNum;
        XInputDotNetPure.GamePad.SetVibration(index, value, value);

        // 1秒後にバイブレーションを止める
        Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(time =>
        {
            if (isVibration == false) return;

            PlayVibration((int)index, false);
        }).AddTo(this);
    }
}
