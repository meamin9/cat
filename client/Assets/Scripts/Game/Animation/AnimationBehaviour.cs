using UnityEngine;

namespace Assets.Scripts.Game.Animation {
    public class AnimationBehaviour : MonoBehaviour {

        private ActController mActCtrl;

        private void OnEnterStatus(AnimationEvent msg) {
            mActCtrl.SetStatus((uint)msg.intParameter);
        }

        private void OnExistStatus(AnimationEvent msg) {
            mActCtrl.ResetStatus((uint)msg.intParameter);
        }

        private void OnSound(AnimationEvent msg) {
            SoundManager.Instance.Play(EAudioChannel.Sounds, msg.stringParameter);
        }

        private void OnEffect(AnimationEvent msg) {

        }
    }
}
