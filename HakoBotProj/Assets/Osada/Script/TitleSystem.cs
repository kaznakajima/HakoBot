using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;
using UniRx.Triggers;

public class TitleSystem : MonoBehaviour
{
    public PlayableDirector m_TiteTimeline;
    public PlayableDirector m_StartTimeline;

    private void Start()
    {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            if (m_TiteTimeline.state != PlayState.Playing && Input.GetMouseButtonDown(0))
            {
                m_StartTimeline.Play();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                m_TiteTimeline.time = 12.5f;
                m_StartTimeline.Play();
            }
        }).AddTo(this);
    }
}
