using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵(エネミー)のベースクラス
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    // 敵のステートマシン
    public enum ENEMY_STATE
    {
        PATROL, // 巡回
        TARGETMOVE // ターゲット追尾
    }
    public ENEMY_STATE state;

    // ナビメッシュ
    [HideInInspector]
    public NavMeshAgent agent;

    // ターゲットとするGameObject
    public GameObject targetObj;

    // ターゲットとの最短距離
    [HideInInspector]
    public float minDistance = 100;

    // アイテムを持つ位置
    public Transform pointPos;

    // 攻撃判定
    [HideInInspector]
    public bool isAttack;

    // 移動スピード
    float runSpeed = 5.0f;

    // 自身のRig
    [HideInInspector]
    public Rigidbody myRig;

    // 自身が持っているアイテム
    [HideInInspector]
    public GameObject itemObj;

    // 巡回地点の一時保存
    [HideInInspector]
    public Vector3 patrolPos;

    /// <summary>
    /// ターゲットのリセット
    /// </summary>
    public virtual void ResetTarget()
    {

    }

    /// <summary>
    /// ターゲットのセット
    /// </summary>
    public virtual void SetTarget()
    {

    }

    /// <summary>
    /// ターゲットになりえるオブジェクトとの距離を取得
    /// </summary>
    public virtual void SearchTarget()
    {

    }

    /// <summary>
    /// ポイントエリアとの距離を取得
    /// </summary>
    public virtual void SearchPointArea()
    {

    }

    /// <summary>
    /// 巡回中の移動
    /// </summary>
    /// <param name="vec">移動先の座標</param>
    public virtual void PatrolMove(Vector3 vec)
    {

    }

    /// <summary>
    /// 巡回する地点の取得
    /// </summary>
    /// <returns>ステージ内のどこかの座標</returns>
    public virtual Vector3 GetRandomPosition()
    {
        // ステージのサイズ
        float stageSize = 4.0f;
        // 巡回用の座標を保存
        patrolPos = new Vector3(Random.Range(-stageSize, stageSize), transform.position.y, Random.Range(-stageSize, stageSize));
        return patrolPos;
    }
}
