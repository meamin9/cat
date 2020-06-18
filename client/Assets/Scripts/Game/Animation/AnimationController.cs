using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Game {
    public class AnimationController : PlayableBehaviour {
        private Playable mMixer;
        private Action<int> mFinishCb;
        private PlayableGraph mGraph;
        public PlayableGraph Graph => mGraph;

        public static AnimationController Create(GameObject go) {
            var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            var graph = PlayableGraph.Create("Action");
            var playable = ScriptPlayable<AnimationController>.Create(graph);
            var animOut = AnimationPlayableOutput.Create(graph, "Animation", animator);
            animOut.SetSourcePlayable(playable, 0);

            var mixer = AnimationMixerPlayable.Create(graph);
            playable.AddInput(mixer, 0, 1);

            var behaviour = playable.GetBehaviour();
            behaviour.mGraph = graph;
            behaviour.mMixer = mixer;
            return behaviour;
        }

        private const float mTransitionDuration = 0.2f;
        private float mTransitionTime;

        private float mActionTime;

        private ulong mFinishFrameId;
        public void PlayAnimation(AnimationClip clip, Action<int> finishCb=null) {
            mFinishCb?.Invoke(-1);
            mFinishCb = finishCb;
            PlayAnimation(clip);
        }
        public void PlayAnimation(AnimationClip clip, AnimationClip clip2, Action<int> finishCb) {
            mFinishCb?.Invoke(-1);
            mFinishCb = (code) => {
                if (code == 0) {
                    mFinishCb = finishCb;
                    PlayAnimation(clip2);
                }
            };
            PlayAnimation(clip);
        }

        private void PlayAnimation(AnimationClip clip) {
            var count = mMixer.GetInputCount();
            if (count > 0) {
                var animPlayable = (AnimationClipPlayable)mMixer.GetInput(count - 1);
                if (animPlayable.GetAnimationClip() == clip) {
                    if (!clip.isLooping) {
                        animPlayable.SetTime(0);
                        mActionTime = clip.length;
                    }
                    if (!mGraph.IsPlaying()) {
                        mGraph.Play();
                    }
                    return;
                }
            }
            var playable = AnimationClipPlayable.Create(mGraph, clip);
            if (!clip.isLooping) {
                mActionTime = clip.length;
            }
            if (count == 0) {
                mMixer.AddInput(playable, 0, 1);
            } else {
                mTransitionTime = mTransitionDuration;
                mMixer.AddInput(playable, 0, 0);
            }
            if (!mGraph.IsPlaying()) {
                mGraph.Play();
            }
        }
        public void ClearAnimation() {
            var count = mMixer.GetInputCount();
            for (var i = 0; i < count; ++i) {
                mMixer.GetInput(i).Destroy();
            }
            mMixer.SetInputCount(0);
            mGraph.Stop();
        }

        override public void PrepareFrame(Playable playable, FrameData info) {
            if (mFinishFrameId == info.frameId) {
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

        public void Destory() {
            mGraph.Destroy();
        }
    }
}
