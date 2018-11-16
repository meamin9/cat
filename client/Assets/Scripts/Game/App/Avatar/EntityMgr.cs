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

        public Entity CreateEntity(string id)
        {
            AssetMgr.Instance.LoadAsync("Entity.prefab", (req) => {
                var entity = new Entity();
                entity.gameObject = GameObject.Instantiate<GameObject>(req.Asset as GameObject);
                entity.OnAttach();
            });
            return null;
        }
    }
}
