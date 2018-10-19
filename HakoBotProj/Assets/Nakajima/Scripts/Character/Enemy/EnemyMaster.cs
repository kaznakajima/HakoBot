using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵AIを管理するクラス
/// </summary>
public class EnemyMaster : MonoBehaviour
{
    // AIの種類
    [SerializeField]
    AttackEnemy attackAI;
    [SerializeField]
    BalanceEnemy blanceAI;
    [SerializeField]
    TransportEnemy transportAI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
