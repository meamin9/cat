using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Base;

namespace Game {

    public static class CameraManager {
        private static TargetFollow _follow;

        static CameraManager() {
            var camera = Camera.main;
            var adapter = camera.gameObject.AddComponent<LateUpdateAdapter>();
            adapter.lateUpdate += LateUpdate;
        }

        public static void StopFollow() {
            _follow = null;
        }

        public static void FollowTarget(Transform target) {
            var camera = Camera.main;
            camera.fieldOfView = Setting.Game.FOV;
            _follow = new TargetFollow(camera.transform, target, Setting.Game.RelativePosition);
        }

        public static void LateUpdate() {
            _follow?.Update();
        }
    }


    public class TargetFollow {
        public Transform _target;
        public Vector3 _offset;
        public Transform _source;

        public TargetFollow(Transform source, Transform target, Vector3 offset) {
            _source = source;
            _target = target;
            _offset = offset;
            _source.position = _target.position + _offset;
        }


        public void Update() {
            _source.position = _target.position + _offset;
        }
    }
}
