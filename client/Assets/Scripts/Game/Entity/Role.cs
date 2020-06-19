using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using Base;

namespace Game
{
    /// <summary>
    /// 战斗属性Key
    /// </summary>
    public static class FightPropKey {
        public const int hp = 0;
        public const int ack = 1;
        public const int def = 2;
    }

    public class FightProperty {
        public float hp;
        public float atk;
        public float def;
        // 属性伤害，火，水，雷，毒
        public float[] extAtk;
        public float[] extDef;


        public float CalcDamage(FightProperty t) {
            return 100f;
            //var dmg = atk - t.def;
            //if (dmg < 0) {
            //    dmg = 1f;
            //}
            //return dmg;
        }

    }

    public class Role {
        public Avatar avatar;
        public MoveController move;

        public ActController act;

        public int id;
        public int guid;
        public tRole table;
        public int skinId;
        public tSkin skin;

        public Skill[] skills;


        private AnimationClip animWalk;
        private AnimationClip animStand;

        public int level;
        public FightProperty fightProps;

        public Role(int id) {
            table = Table<tRole>.Find(id);
            Log.ErrorIf(table == null, $"找不到 RoleTable :{id}");
            this.id = id;
            guid = Funcs.NewGuid();
            skinId = table.skinIds[0];

            skin = Table<tSkin>.Find(skinId);
            avatar = new Avatar(SceneManager.roleRoot, skin.skeleton, skin.skin, AfterRoleLoadFinish);
            avatar.gameObject.name = id + "_" + guid;

            act = new ActController(this);

            move = new MoveController(avatar.gameObject.transform);
            move.onMoveFinished = OnMoveFinished;
            move.onMoveStart = OnMoveStart;

            //加载动画
            AssetMgr.Instance.LoadAsync(skin.animDir + "/Stand.anim", (anim) => {
                animWalk = anim as AnimationClip;
            });
            AssetMgr.Instance.LoadAsync(skin.animDir + "/Stand.anim", (anim) => {
                animStand = anim as AnimationClip;
                AfterRoleLoadFinish();
            });
            LoadAnimationClips(act.actCombos);
        }

        public void LoadAnimationClips(Act[] acts) {
            var dir = skin.animDir;
            foreach (var act in acts) {
                AssetMgr.Instance.LoadAsync(dir + act.table.anim, (anim) => act.anim = anim as AnimationClip);
                if (act.next != null) {
                    LoadAnimationClips(act.next);
                }
            }
        }

        public void AfterRoleLoadFinish() {
            if (animStand != null && avatar.isLoaded) {
                avatar.AnimCtrl.PlayAnimation(animStand);
                avatar.AnimEventAdapter.SetActListener(act);
            }

        }

        public Skill GetSkill(int skillIndex) {
            return skills[skillIndex];
        }

        public void OnMoveFinished() {
            avatar.AnimCtrl.PlayAnimation(animStand);
        }

        public void OnMoveStart() {
            avatar.AnimCtrl.PlayAnimation(animWalk);
        }

        public bool CanSelected() {
            return true;
        }


    }

    public class EntityObject
    {
        public GameObject gameObject { get; set; }
        //public MoveController MoveCtrl { get; set; }
        //public AnimController AnimCtrl { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }

        public Dictionary<string, object> Extra;
        


        public void Update()
        {
            //MoveCtrl.Update();
            //if (Input.GetMouseButtonDown(0))
            //{
            //    RaycastHit hitInfo;
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            //        _navAgent.destination = hitInfo.point;
            //}
        }
        

    }
}
