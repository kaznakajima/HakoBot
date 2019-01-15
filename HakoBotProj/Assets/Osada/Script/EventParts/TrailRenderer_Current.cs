using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderer_Current : MonoBehaviour
{
    //
    private TrailRenderer m_TrailRenderer;

    /// <summary>
    /// TrailRendererに移動する座標を順番に設定する
    /// </summary>
    /// <param name="pos">設定する座標</param>
    public void SetPosition(Vector3[] pos)
    {
        m_TrailRenderer.SetPositions(pos);
    }
}
