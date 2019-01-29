using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class AI_Poibot : MonoBehaviour
{
    public enum ItemType
    {
        Baggage,
        HighBaggage,
        Missile,
        Keibot
    }
    //投てき物用構造体
    [System.Serializable]
    public class Item
    {
        [Header("投てき物")]
        public GameObject m_ItemObj;
        [Header("アイテムタイプ")]
        public ItemType m_ItemType;
        [Header("投げていいものかの判断（触るの禁止）")]
        public bool m_Event;
    }
    [SerializeField,Header("投てき物リスト")]
    public List<Item> m_Item;
    [SerializeField,Header("投てき物出現地点")]
    private Transform m_GenerationPosition;

    [SerializeField,Header("投てき位置")]
    private Vector3 m_ThrowPosition;
    [SerializeField, Header("補充位置")]
    private Vector3 m_PreparationPosition;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 1.0f;

    private bool m_Throwing;

    [SerializeField, Header("横の半径")]
    private float m_SizeX;
    [SerializeField, Header("奥行の半径")]
    private float m_SizeZ; 

    private void Start()
    {
        PrepareForThrowing();
    }

    private void PrepareForThrowing()
    {
        if (MainManager.Instance.isStop)
        {
            StopAllCoroutines();
            return;
        }

        m_Throwing = true;

        var objList = m_Item.Where(c => c.m_Event).ToList();
        var objNumber = Random.Range(0, objList.Count());

        var targetPos = GetCoordinate();
        var obj = Instantiate(m_Item[objNumber].m_ItemObj, m_GenerationPosition.position, transform.rotation);
        obj.transform.parent = transform;

        switch (m_Item[objNumber].m_ItemType)
        {
            case ItemType.Baggage:
            case ItemType.HighBaggage:
                Rigidbody rid = obj.GetComponent<Rigidbody>();
                // 射出速度を算出
                Vector3 velocity = CalculateVelocity(this.transform.position, targetPos, 60.0f);
                Observable.FromCoroutine(Move, publishEveryYield: false).
                    Subscribe(_ =>
                    {
                        if (MainManager.Instance.isStop)
                        {
                            StopAllCoroutines();
                            return;
                        }

                        var collider = obj.GetComponent<Collider>();
                        collider.isTrigger = false;
                        obj.transform.parent = null;
                        rid.useGravity = true;
                        // 射出
                        rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
                        StartCoroutine(Move());
                    });
                break;
            case ItemType.Missile:
                Observable.FromCoroutine(Move, publishEveryYield: false).
                    Subscribe(_ =>
                    {
                        if (MainManager.Instance.isStop)
                        {
                            StopAllCoroutines();
                            return;
                        }

                        var collider = obj.GetComponent<Collider>();
                        collider.isTrigger = false;
                        var missile = obj.GetComponent<Missile>();
                        missile.Setting(targetPos);
                        obj.transform.parent = null;
                        StartCoroutine(Move());
                    });
                break;
            case ItemType.Keibot:
                break;
        }
    }

    private IEnumerator Move()
    {
        var position = m_Throwing ? m_ThrowPosition : m_PreparationPosition;
        while (true)
        {
            var dir = (position - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, position);
            var currentVelocity = m_Velocity;
            m_Velocity = dis > 0.3f ? dir * m_Speed : Vector3.zero;
            m_Velocity = Vector3.Lerp(currentVelocity, m_Velocity, Mathf.Min(Time.deltaTime + 5.0f, 1));


            Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, 360.0f * Time.deltaTime);
            transform.position += m_Velocity * Time.deltaTime;

            if (dis < 0.3f)
                break;

            yield return null;
        }
        if (m_Throwing)
            m_Throwing = false;
        else
            PrepareForThrowing();
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;
        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
        // 垂直方向の距離y
        float y = pointA.y - pointB.y;
        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));
        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    public Vector3 GetCoordinate()
    {
        var x = Random.Range(-m_SizeX, m_SizeX);
        var z = Random.Range(-m_SizeZ, m_SizeZ);

        var targetPos = new Vector3(x, 0.2f, z);

        return targetPos;
    }
}
