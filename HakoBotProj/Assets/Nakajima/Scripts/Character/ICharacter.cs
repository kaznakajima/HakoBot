using UnityEngine;

/// <summary>
/// キャラクターが持つインターフェイス
/// </summary>
public interface Character
{
    // プレイヤー番号
    int myNumber { get; set; }

    // エネルギー残量
    int myEnergy { get; set; }

    // アイテムを所持しているか
    bool hasItem { get; set; }

    // オーバーヒートしたか
    bool isStan { get; set; }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    void Move(Vector3 vec);

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

    /// <summary>
    /// アイテムを放棄
    /// </summary>
    /// <param name="isSteal">アイテムを奪うかどうか</param>
    /// <param name="opponentPos">ぶつかってきたプレイヤーの座標</param>
    void Release(bool isSteal, Vector3 opponentPos);

    /// <summary>
    /// 荷物配達完了
    /// </summary>
    void ItemCarry();
}