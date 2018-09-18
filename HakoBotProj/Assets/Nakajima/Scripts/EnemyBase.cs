using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵(エネミー)のベースクラス
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    // ナビメッシュ
    [HideInInspector]
    public NavMeshAgent agent;

    // ターゲットとするGameObject
    public GameObject targetObj;

    // 最短距離
    [HideInInspector]
    public float minDistance = 100;

    // アイテムを所持するための座標
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

    public virtual void ResetTarget()
    {

    }

    public virtual void SetTarget()
    {

    }


}
