using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Event_CurrentPanel : Event
{
    [SerializeField]
    private Panel[] m_Panel;

    public override void EventStart()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(5.0f))
            .Subscribe(_ =>
            {
                foreach (Panel panel in m_Panel)
                {
                    panel.m_Activation.Value = false;
                }

                var repeatCount = Random.Range(1, 4);
                for(int i = 0; i < repeatCount; ++i)
                {
                    PanelSearch();
                }
            }).AddTo(this);
    }

    public void PanelSearch()
    {
        var number = Random.Range(0, m_Panel.Length);

        if (m_Panel[number].m_Activation.Value)
            m_Panel[number].m_Activation.Value = true;
        else
            PanelSearch();
    }
}
