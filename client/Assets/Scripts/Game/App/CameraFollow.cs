using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Automata.Base;
using Automata.Adapter;

namespace Automata.Game
{

    public class CameraMgr : Singleton<CameraMgr>
    {

        private UpdateAdapter _adapter;
        public void Initialize()
        {
            SceneMgr.Instance.OnSceneLoaded += FollowPlayer;
            EntityMgr.Instance.OnPlayerLoaded += FollowPlayer;
        }

        public void FollowPlayer()
        {
            if (EntityMgr.Player == null || Camera.main == null)
            {
                return;
            }
            var camera = Camera.main;
            camera.fieldOfView = AppSetting.Instance.FOV;
            var follow = new CameraFollow(camera, EntityMgr.Player.gameObject.transform, 
                AppSetting.Instance.RelativePosition);
            var adapter = camera.GetComponent<LateUpdateAdapter>();
            if (adapter != null)
            {
                adapter.enabled = false;
                GameObject.Destroy(adapter);
            }
            adapter = camera.gameObject.AddComponent<LateUpdateAdapter>();
            adapter.lateUpdate += follow.LateUpdate;

        }

        public void Follow(Transform target)
        {
            //var offset = AppSetting.Instance.RelativePosition;
            //var follow = new CameraFollow(target, offset, MainCamera);
            //_adapter.update = follow.Update;
        }
    }


    public class CameraFollow
    {
        public Transform _target;
        public Vector3 _relativePosition;
        public Camera _camera;

        public CameraFollow(Camera camera, Transform target, Vector3 offset)
        {
            _target = target;
            _relativePosition = offset;
            _camera = camera;
            _camera.transform.position = target.position + _relativePosition;
        }


        public void LateUpdate()
        {
            var pos = _target.position + _relativePosition;
            _camera.transform.position = pos;
        }
    }
}
