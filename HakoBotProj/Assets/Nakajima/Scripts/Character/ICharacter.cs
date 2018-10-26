﻿using UnityEngine;

/// <summary>
/// キャラクターが持つインターフェイス
/// </summary>
public interface Character
{
    // プレイヤー番号
    int myNumber { get; set; }

    // エネルギー残量
    int myEnergy { get; set; }

    // チャージ段階
    int chargeLevel { get; set; }

    // アイテムを所持しているか
    bool hasItem { get; set; }
    
    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    void Move(Vector3 vec);

    // チャージ
    void Charge();

    // タックル
    void Attack();

    // ジャンプ
    void Jump();

    // スタン
    void Stan();

    /// <summary>
    /// アイテムの取得
    /// </summary>
    /// <param name="obj">アイテムのオブジェクト</param>
    void Catch(GameObject obj);

    // アイテムを放棄
    void Release();
}