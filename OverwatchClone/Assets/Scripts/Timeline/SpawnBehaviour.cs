using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SpawnBehaviour : PlayableBehaviour {
    //public string characterName;
    public SpawnWaveData waveData;
    public SpawnMaster spawnMaster;
    //public int dialogueSize;

    //public bool hasToPause = false;

    private bool clipPlayed = false;
    //private bool pauseScheduled = false;
    private PlayableDirector director;
    bool runOnce = true;

    public override void OnPlayableCreate(Playable playable) {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        if (!Application.isPlaying)
            return;
        if (!clipPlayed && info.weight > 0f) {

            spawnMaster.SpawnWave(waveData);


            if (Application.isPlaying) {

                //if(hasToPause)
                //{
                //	pauseScheduled = true;
                //}
            }

            clipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info) {
        //if(pauseScheduled)
        //{
        //	pauseScheduled = false;
        //          // TODO?
        //	//GameManager.Instance.PauseTimeline(director);
        //}
        //else
        //{
        //    // UIManager.Instance.ToggleDialoguePanel(false);
        //    Subtitles.instance.SetSubtitle("");
        //}
        clipPlayed = false;
    }
}
