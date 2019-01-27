using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PlayableTrack_Result : PlayableAsset
{
    // シーン上のオブジェクトはExposedReference<T>を使用する
    public ExposedReference<Sample> m_Ranking;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var behaviour = new ResultText();
        behaviour.m_Ranking = m_Ranking.Resolve(graph.GetResolver());
        return ScriptPlayable<ResultText>.Create(graph, behaviour);
    }
}
