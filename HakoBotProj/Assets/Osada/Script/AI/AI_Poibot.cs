using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AI_Poibot : MonoBehaviour
{
    //投てき用データ
    [SerializeField]
    private PoiBotData poiBotData;
    //荷物管理用
    [SerializeField]
    private ItemManagement itemManagement;

    [SerializeField, Header("投てき物出現地点")]
    private Transform throwing_object_pop_point;

    [SerializeField, Header("投てき位置")]
    private Vector3 throwing_position;
    [SerializeField, Header("補充位置")]
    private Vector3 replenishment_position;

    //速度保存用
    private Vector3 velocity = Vector3.zero;
    //移動速度
    private float speed = 1.0f;
    //投てき可能か判断
    private bool throwing_possible;

    private BoolReactiveProperty startup = new BoolReactiveProperty(false);

    private void Start()
    {
        this.UpdateAsObservable().
            Subscribe(_ =>
            {
                if (!startup.Value && itemManagement.get_list_factor_count() <= 7)
                    startup.Value = true;
            }).AddTo(this);

        startup.Where(c => c).
            Subscribe(c =>
            {
                PrepareForThrowing();
            }).AddTo(this);
    }

    private void PrepareForThrowing()
    {
        if (MainManager.Instance.isStop)
        {
            StopAllCoroutines();
            return;
        }

        if (itemManagement.get_list_factor_count() > 7)
        {
            startup.Value = false;
            return;
        }

        throwing_possible = true;

        var objList = poiBotData.item.Where(c => c.valid).ToList();
        var objNumber = Random.Range(0, objList.Count());

        var targetPos = GetCoordinate();
        var obj = Instantiate(poiBotData.item[objNumber].items_object, throwing_object_pop_point.position, transform.rotation);
        itemManagement.add_to_list(obj);
        obj.transform.parent = transform;

        switch (poiBotData.item[objNumber].itemType)
        {
            case PoiBotData.ItemType.Baggage:
            case PoiBotData.ItemType.HighBaggage:
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
        }
    }

    private IEnumerator Move()
    {
        var position = throwing_possible ? throwing_position : replenishment_position;
        while (true)
        {
            var dir = (position - transform.position).normalized;
            var dis = Vector3.Distance(transform.position, position);
            var currentVelocity = velocity;
            velocity = dis > 0.3f ? dir * speed : Vector3.zero;
            velocity = Vector3.Lerp(currentVelocity, velocity, Mathf.Min(Time.deltaTime + 5.0f, 1));


            Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, 360.0f * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;

            if (dis < 0.3f)
                break;

            yield return null;
        }
        if (throwing_possible)
            throwing_possible = false;
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
    /// <summary>
    /// 投てき座標を取得
    /// </summary>
    /// <returns>投てき座標</returns>
    public Vector3 GetCoordinate()
    {
        var x = Random.Range(-poiBotData.throwing_range_x, poiBotData.throwing_range_x);
        var z = Random.Range(-poiBotData.throwing_range_z, poiBotData.throwing_range_z);

        var targetPos = new Vector3(x, 0.2f, z);

        return targetPos;
    }
}
