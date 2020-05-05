using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public enum ETrack {
    BGM,
    Sounds,

    Count
}

public class SoundManager : PlayableBehaviour {
    private Playable mMixer;
    private Action<int> mFinishCb;
    private ScriptPlayable<SoundManager> mPlayable;
    private PlayableGraph mGraph;
    public PlayableGraph Graph => mGraph;

    private Playable[] mTracks;
    public static SoundManager Create(GameObject go) {
        var audioSource = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();
        var graph = PlayableGraph.Create("AudioPlayable");
        var audioOut = AudioPlayableOutput.Create(graph, "Audio", audioSource);

        var playable = ScriptPlayable<SoundManager>.Create(graph);
        audioOut.SetSourcePlayable(playable, 0);

        var trackCount = (int)ETrack.Count;
        var mixer = AudioMixerPlayable.Create(graph, trackCount);
        playable.AddInput(mixer, 0, 1);
        for (var i = 0; i < trackCount; ++i) {
            mixer.AddInput(AudioMixerPlayable.Create(graph), i, 1);
        }

        var behaviour = playable.GetBehaviour();
        behaviour.mGraph = graph;
        behaviour.mPlayable = playable;
        behaviour.mMixer = mixer;
        return behaviour;
    }

    private float mTransitionDuration = 0.2f;
    private float mTransitionTime;

    private float mActionTime;

    private ulong mFinishFrameId;

    override public void PrepareFrame(Playable owner, FrameData info) {
        if (mFinishFrameId == info.frameId) {
            mFinishFrameId = 0;
            var cb = mFinishCb;
            mFinishCb = null;
            cb.Invoke(0);
            return;
        }
        var count = mMixer.GetInputCount();
        if (count == 0) {
            return;
        }
        if (count > 1 && mTransitionTime >= 0f) {
            mTransitionTime -= info.deltaTime;
            if (mTransitionTime > 0f) {
                var rate = mTransitionTime / mTransitionDuration;
                for (var i = 0; i < count - 1; ++i) {
                    mMixer.SetInputWeight(i, Mathf.Lerp(mMixer.GetInputWeight(i), 0f, rate));
                }
                mMixer.SetInputWeight(count - 1, Mathf.Lerp(mMixer.GetInputWeight(count - 1), 1f, 1f - rate));
            } else {
                for (var i = 0; i < count - 1; ++i) {
                    mMixer.GetInput(i).Destroy();
                }
                var clipPlayable = mMixer.GetInput(count - 1);
                mMixer.DisconnectInput(count - 1);
                mMixer.SetInputCount(1);
                mMixer.ConnectInput(0, clipPlayable, 0);
                mMixer.SetInputWeight(0, 1.0f);
            }
        }
        if (mActionTime > 0) {
            mActionTime -= info.deltaTime;
            if (mActionTime <= 0 && mFinishCb != null) {
                mFinishFrameId = info.frameId + 2;

            }
        }
    }
    public void LoadSound(string audioName) {

    }

    public void Play(ETrack track, string audioName) {

    }

    public void Play(ETrack track, AudioClip audioClip) {
        var playable = mMixer.GetInput((int)track);
        if (track == ETrack.BGM) {
            //playable.GetInputCount()
            //foreach (var p in ) {

            //}
        }
    }

    public void SetVolume(ETrack track, float volume) {
        mMixer.SetInputWeight((int)track, volume);
    }

    public void SetMute(ETrack track, bool mute) {
        SetVolume(track, mute ? 1 : 0);
    }
    
}
 