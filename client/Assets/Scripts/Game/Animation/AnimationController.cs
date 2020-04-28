using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

//namespace Assets.Scripts.Game.Animation {
public enum ActionStatus {
    Normal,
    BaTi,
    HouBai,

}

public class ActionController: PlayableBehaviour {
    private Playable mMixer;
    private Action<int> mFinishCb;
    private ScriptPlayable<ActionController> mPlayable;
    private PlayableGraph mGraph;
    public PlayableGraph Graph => mGraph;

    public static ActionController Create(GameObject go) {
        var animator = go.GetComponent<Animator>();
        if (animator == null) {
            animator = go.AddComponent<Animator>();
        }
        var graph = PlayableGraph.Create();
        var playable = ScriptPlayable<ActionController>.Create(graph);
        var animOut = AnimationPlayableOutput.Create(graph, "Animation", animator);
        animOut.SetSourcePlayable(playable, 0);

        var mixer = AnimationMixerPlayable.Create(graph);
        playable.AddInput(mixer, 0, 1);

        var behaviour = playable.GetBehaviour();
        behaviour.mGraph = graph;
        behaviour.mPlayable = playable;
        behaviour.mMixer = mixer;
        return behaviour;
    }

    private float mTransitionDuration = 0.2f;
    private float mTransitionTime;

    private float mActionLength;
    public void PlayAnimation(AnimationClip clip, Action<int> finishCb) {
        mFinishCb?.Invoke(-1);
        mFinishCb = finishCb;
        PlayAnimation(clip);
    }
    public void PlayAnimation(AnimationClip clip, AnimationClip clip2, Action<int> finishCb) {
        mFinishCb?.Invoke(-1);
        mFinishCb = finishCb;
        PlayAnimation(clip);
    }

    public void PlayAnimation2(AnimationClip clip, AnimationClip clip2, AnimationClip clip3, Action<int> finishCb) {
        mFinishCb?.Invoke(-1);
        mFinishCb = finishCb;
        PlayAnimation(clip);
    }
    private void PlayAnimation(AnimationClip clip) {
        var count = mMixer.GetInputCount();
        if (count == 0) {
            mMixer.AddInput(AnimationClipPlayable.Create(mGraph, clip), 0, 1);
            return;
        }
        mTransitionTime = mTransitionDuration;
        mMixer.AddInput(AnimationClipPlayable.Create(mGraph, clip), 0, 0);
    }

    public void Initialize(AnimationClip[] clipsToPlay, Playable owner, PlayableGraph graph) {

        owner.SetInputCount(1);

        mixer = AnimationMixerPlayable.Create(graph, clipsToPlay.Length);

        graph.Connect(mixer, 0, owner, 0);

        owner.SetInputWeight(0, 1);

        for (int clipIndex = 0; clipIndex < mixer.GetInputCount(); ++clipIndex) {
            graph.Connect(AnimationClipPlayable.Create(graph, clipsToPlay[clipIndex]), 0, mixer, clipIndex);
            mixer.SetInputWeight(clipIndex, 1.0f);
        }

    }

    override public void PrepareFrame(Playable owner, FrameData info) {
        var count = mMixer.GetInputCount();
        if (count == 0) {
            return;
        }
        var clipPlayable = mMixer.GetInput(count - 1);
        if (count > 1 && mTransitionTime > 0f) {
            mTransitionTime -= info.deltaTime;
            if (mTransitionTime > 0f) {
                var rate = mTransitionTime / mTransitionDuration;
                for (var i = 0; i < count - 1; ++i) {
                    mMixer.SetInputWeight(i, Mathf.Lerp(mMixer.GetInputWeight(i), 0f, rate));
                }
                mMixer.SetInputWeight(count - 1, Mathf.Lerp(mMixer.GetInputWeight(count - 1), 1f, 1f - rate));
            } 
            else {
                for (var i = 0; i < count - 1; ++i) {
                    mMixer.GetInput(i).Destroy();
                }
                mMixer.SetInputCount(1);
                mMixer.ConnectInput(0, clipPlayable, 0);
                mMixer.SetInputWeight(0, 1.0f);
            }
        }
        var time = clipPlayable.GetTime();
        var duration = clipPlayable.GetDuration();
        if (time >= duration) {
            mFinishCb?.Invoke(0);
        }
        if (time + mTransitionDuration > duration) {

        }

        // Advance to next clip if necessary
        //mActionLength -= info.deltaTime;

        //if (m_TimeToNextClip <= 0.0f) {
        //    m_CurrentClipIndex++;
        //    if (m_CurrentClipIndex == mixer.GetInputCount()) {
        //        var clip = (AnimationClipPlayable)mixer.GetInput(m_CurrentClipIndex);
        //        if (clip.GetAnimationClip().isLooping) {
        //            return;
        //        }
        //    }
        //        m_CurrentClipIndex = 0;
        //    var currentClip = (AnimationClipPlayable)mixer.GetInput(m_CurrentClipIndex);
        //    // Reset the time so that the next clip starts at the correct position
        //    currentClip.SetTime(0);
        //    m_TimeToNextClip = currentClip.GetAnimationClip().length;

        //    // Adjust the weight of the inputs
        //    for (int clipIndex = 0; clipIndex < mixer.GetInputCount(); ++clipIndex) {
        //        if (clipIndex == m_CurrentClipIndex)
        //            mixer.SetInputWeight(clipIndex, 1.0f);
        //        else
        //            mixer.SetInputWeight(clipIndex, 0.0f);
        //    }
        //}
    }
}
//}
