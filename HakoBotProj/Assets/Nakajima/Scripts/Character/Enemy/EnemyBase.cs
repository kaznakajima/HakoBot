using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵AIのベースクラス
/// 敵AIが共通でつ処理、変数はここに書く
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
    
    // 自身のレイヤー番号
    [HideInInspector]
    public int layerNum;

    // 自身のAudio関連
    public AudioSource myAudio;

    // ターゲットオブジェクトのリスト
    public List<PointArea> targetList = new List<PointArea>();

    // ナビメッシュ
    [HideInInspector]
    public NavMeshAgent agent;

    // ターゲットとするGameObject
    public GameObject targetObj;

    // 消去法ターゲット
    [HideInInspector]
    public GameObject dummyTarget;

    // ターゲットとの最短距離
    [HideInInspector]
    public float minDistance = 100;

    // ターゲットとの最長距離
    [HideInInspector]
    public float maxDistance = 0;

    // アイテムを持つ位置
    public Transform pointPos;

    // 攻撃判定
    [HideInInspector]
    public bool isAttack;

    // 自身のRig
    [HideInInspector]
    public Rigidbody myRig;

    // 自身が持っているアイテム
    [HideInInspector]
    public GameObject itemObj;

    // エフェクト用オブジェクト
    protected GameObject stanEffect;

    // 巡回地点の一時保存
    [HideInInspector]
    public Vector3 patrolPos;

    /// <summary>
    /// ステージ上のすべてのプレイヤーを取得
    /// </summary>
    /// <returns>Playerクラスの配列</returns>
    public virtual GameObject[] GetCharacter()
    {
        return GameObject.FindGameObjectsWithTag("Character");
    }

    /// <summary>
    /// ステージ上のすべてのアイテムを取得
    /// </summary>
    /// <returns>Itemクラスの配列</returns>
    public virtual Item[] GetItems()
    {
        return FindObjectsOfType<Item>();
    }

    /// <summary>
    /// ステージ上のすべてのゴールを取得
    /// </summary>
    /// <returns>PointAreaクラスの配列</returns>
    public virtual PointArea[] GetPointArea()
    {
        return FindObjectsOfType<PointArea>();
    }

    /// <summary>
    /// ターゲットの状態を取得
    /// </summary>
    /// <param name="otherObj">他のプレイヤー</param>
    public virtual void CheckTarget(GameObject otherObj)
    {

    }

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
    /// 自身とターゲットとの距離を計算
    /// </summary>
    /// <param name="targetObj">ターゲットのオブジェクト</param>
    /// <param name="myObj">自身のオブジェクト</param>
    /// <returns></returns>
    public float GetTargetDistance(GameObject targetObj, GameObject myObj)
    {
        float distance = Vector3.Distance(targetObj.transform.position, myObj.transform.position);
        return distance;
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

    public virtual void OnCollisionExit(Collision col)
    {
        // タックル中にプレイヤーに触れたとき
        if (col.gameObject.GetComponent(typeof(Character)) as Character != null)
        {
            myRig.velocity = Vector3.zero;
        }
    }
}
