using UnityEngine;

/// <summary>
/// キャラクターが持つインターフェイス
/// </summary>
public interface Character
{
    int number { get; set; }

    bool item { get; set; }

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

    // アイテムを放棄
    void Release();

    // 充電
    void Charge();
}