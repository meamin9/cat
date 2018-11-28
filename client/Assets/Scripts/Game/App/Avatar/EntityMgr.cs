using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automata.Base;
using UnityEngine;

namespace Automata.Game
{
    public class EntityMgr : Singleton<EntityMgr>
    {
        public static Player Player { get; private set; }

        public event System.Action OnPlayerLoaded;

        public Entity CreatePlayer(string id)
        {
            AssetMgr.Instance.LoadAsync("unitychan.prefab", (req) => {
                var entity = new Player();
                entity.gameObject = GameObject.Instantiate<GameObject>(req.Asset as GameObject);
                entity.OnAttach();
                Player = entity;
                OnPlayerLoaded?.Invoke();
            });
            return null;
        }
    }
}
