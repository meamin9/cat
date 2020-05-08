using AM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public enum EAudioChannel {
    BGM,
    Sounds,

    Count
}

public class SoundManager : PlayableBehaviour {
    private Playable mMixer;
    private ScriptPlayable<SoundManager> mPlayable;
    private PlayableGraph mGraph;
    public PlayableGraph Graph => mGraph;

    public static SoundManager Create(GameObject go) {
        var audioSource = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();
        var graph = PlayableGraph.Create("AudioPlayable");
        var audioOut = AudioPlayableOutput.Create(graph, "Audio", audioSource);

        var playable = ScriptPlayable<SoundManager>.Create(graph);
        audioOut.SetSourcePlayable(playable, 0);

        var trackCount = (int)EAudioChannel.Count;
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
    #region AudioClip Load
    public void LoadAudio(string name) {
        AssetMgr.Instance.LoadAsync(name, (asset) => {
        });
    }
    #endregion
    #region BGM
    private float mSingleTransitionOut;
    private float mSingleTransitionIn;
    private const float mSingleTransitionTime = 1.5f;
    private AudioClip mBgmAudioClip;

    public void PlayBGM(AudioClip audioClip) {
        mBgmAudioClip = audioClip;
        var bgm = mMixer.GetInput((int)EAudioChannel.BGM);
        if (IsMute(EAudioChannel.BGM)) {
            return;
        }
        if (bgm.GetInputCount() == 0) {
            bgm.AddInput(AudioClipPlayable.Create(mGraph, audioClip, true), 0, 0);
            mSingleTransitionIn = mSingleTransitionTime;
            return;
        }
        mSingleTransitionOut = mSingleTransitionIn = mSingleTransitionTime;
    }

    private void PrepareSingleTrack(Playable owner, float deltaTime) {
        var count = owner.GetInputCount();
        if (count == 0) {
            return;
        }
        if (mSingleTransitionOut > 0f) {
            mSingleTransitionOut -= deltaTime;
            var t = Mathf.Clamp01(1 - mSingleTransitionOut / mSingleTransitionTime);
            var weight = owner.GetInputWeight(0);
            owner.SetInputWeight(0, Mathf.Lerp(weight, 0, t));
        }
        else if (mSingleTransitionIn > 0f && mBgmAudioClip != null) {
            var playable = (AudioClipPlayable)owner.GetInput(0);
            if (playable.GetClip() != mBgmAudioClip) {
                playable.SetClip(mBgmAudioClip);
                playable.SetTime(0);
            }
            mSingleTransitionIn -= deltaTime;
            var t = Mathf.Clamp01(1 - mSingleTransitionIn / mSingleTransitionTime);
            var weight = owner.GetInputWeight(0);
            owner.SetInputWeight(0, Mathf.Lerp(weight, 1, t));
        }
    }

    override public void PrepareFrame(Playable owner, FrameData info) {
        var count = mMixer.GetInputCount();
        if (count == 0) {
            return;
        }
        PrepareSingleTrack(mMixer.GetInput((int)EAudioChannel.BGM), info.deltaTime);
    }
    #endregion



    public void Play(EAudioChannel channel, string audioName) {

    }

    public void Play(EAudioChannel channel, AudioClip audioClip) {
        if (channel == EAudioChannel.BGM) {
            PlayBGM(audioClip);
            return;
        }
        if (IsMute(channel)) {
            return;
        }
        var playable = mMixer.GetInput((int)channel);
        var count = playable.GetInputCount();
        for(var i = 0; i < count; ++i) {
            var t = (AudioClipPlayable)playable.GetInput(i);
            if (t.IsDone()) {
                t.SetClip(audioClip);
                t.SetTime(0);
                t.SetDuration(audioClip.length);
                t.SetDone(false);
                return;
            }
        }
        var p = AudioClipPlayable.Create(mGraph, audioClip, false);
        p.SetDuration(audioClip.length);
        playable.AddInput(p, 0, 1);
    }

    public void SetVolume(EAudioChannel channel, float volume) {
        mMixer.SetInputWeight((int)channel, volume);
    }

    public void SetMute(EAudioChannel channel, bool mute) {
        var playable = mMixer.GetInput((int)channel);
        if (mute) {
            playable.DestoryAllInput();
            playable.Pause();
        }
        else {
            mMixer.GetInput((int)channel).Play();
        }
    }

    public bool IsMute(EAudioChannel channel) {
        return mMixer.GetInput((int)channel).GetPlayState() == PlayState.Paused;
    }
    
}
 