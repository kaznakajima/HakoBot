using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MarkerSystem : MonoBehaviour
{
    private float m_MinSize = 0.1f;
    private float m_MaxSize = 0.15f;

    private void Start()
    {
        var size = m_MaxSize;
        var add = 0.001f;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var scale = transform.localScale.x;
                scale = size == m_MaxSize ? scale + add : scale -= add;
                transform.localScale = new Vector3(scale, scale, 0.1f);

                if (scale > m_MaxSize || scale < m_MinSize)
                    size = size == m_MaxSize ? m_MinSize : m_MaxSize;

                add += 0.001f * Time.deltaTime;
            }).AddTo(this);

        Observable.Timer(System.TimeSpan.FromSeconds(5.0f))
            .Subscribe(_ =>
            {
                Destroy(gameObject);
            }).AddTo(this);
    }
}
