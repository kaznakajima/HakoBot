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

    // ターゲットとされているか
    bool isTarget { get; set; }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="vec">移動方向</param>
    void Move(Vector3 vec);

    // タックル
    void Attack();

    // スタン
    void Stan(string audioStr);

    /// <summary>
    /// アイテムの取得
    /// </summary>
    /// <param name="obj">アイテムのオブジェクト</param>
    void Catch(GameObject obj);

    /// <summary>
    /// アイテムを放棄
    /// </summary>
    void Release();

    /// <summary>
    /// 荷物配達完了
    /// </summary>
    void ItemCarry();

    /// <summary>
    /// 自身のレイヤーを変更する
    /// </summary>
    /// <param name="layerNum">レイヤー番号</param>
    void LayerChange(int layerNum);
}