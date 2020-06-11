using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;
using UnityEngine;

namespace Game
{
    public class EntityManager : Singleton<EntityManager>
    {
        public static Player Player { get; private set; }

        public event System.Action OnPlayerLoaded;

        public EntityObject CreateEntity(int id)
        {
            //var entry = ConfMgr<EntityEntry>.Find(id);
            //AssetMgr.Instance.LoadAsync(entry.Prefab, (req) => {
            //    var entity = new Player();
            //    entity.gameObject = GameObject.Instantiate<GameObject>(req.Asset as GameObject);
            //    entity.OnAttach();
            //    Player = entity;
            //    OnPlayerLoaded?.Invoke();
            //});
            return null;
        }
    }
}
