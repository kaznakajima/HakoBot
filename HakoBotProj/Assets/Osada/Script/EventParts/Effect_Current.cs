using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Current : MonoBehaviour
{
    [SerializeField]
    private GameObject m_EffectPre;

    public void GenerateEffects(GameObject from, GameObject end)
    {
        //距離計算
        var distance = Vector3.Distance(from.transform.position, end.transform.position);
        //対象の方向を計算
        var direction = Quaternion.LookRotation(end.transform.position - from.transform.position);
        //出現地点に生成を行い、目標地点を向かせる
        var effect = Instantiate(m_EffectPre, from.transform.position, direction) as GameObject;

        //サイズを設定する
        //サイズ計算
        var size = (distance - 8) * -1;
    }
}
