using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SpawnClip : PlayableAsset, ITimelineClipAsset {
    public SpawnBehaviour template = new SpawnBehaviour();

    public ClipCaps clipCaps {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<SpawnBehaviour>.Create(graph, template);

        return playable;
    }
}
