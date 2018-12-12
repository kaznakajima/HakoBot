using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Sample : Event
{
    private GameObject m_Marker;

    private void MarkerSetting(Vector3 targetPos)
    {
        Instantiate(m_Marker, targetPos, transform.rotation);
    }
}
