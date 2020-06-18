using UnityEngine;

namespace Game {
    public class AnimationEventAdapter : MonoBehaviour {

        private ActController mActCtrl;

        public void SetActListener(ActController act) {
            this.mActCtrl = act;
        }

        private void OnDamage(AnimationEvent msg) {
            mActCtrl?.OnActJudge(msg.intParameter);
        }

        private void OnEnterStatus(AnimationEvent msg) {
            mActCtrl?.SetStatus((uint)msg.intParameter);
        }

        private void OnExistStatus(AnimationEvent msg) {
            mActCtrl?.ResetStatus((uint)msg.intParameter);
        }

        private void OnSound(AnimationEvent msg) {
            SoundManager.Instance.Play(EAudioChannel.Sounds, msg.stringParameter);
        }

        private void OnEffect(AnimationEvent msg) {

        }
    }
}
