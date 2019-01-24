using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_NewPoibot : MonoBehaviour
{
    [SerializeField]
    private PoibotData m_PoibotData;

    [SerializeField, Header("投てき物出現地点")]
    private Transform m_GenerationPosition;

    [SerializeField, Header("投てき位置")]
    private Vector3 m_ThrowPosition;
    [SerializeField, Header("補充位置")]
    private Vector3 m_PreparationPosition;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_Speed = 1.0f;

    private bool m_Throwing;
}
