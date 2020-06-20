using Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public enum EActStatus {
        Idle = 0x0,
        /// <summary>
        /// 是否处于霸体，非霸体受到攻击会被打断
        /// </summary>
        Stoic = 0x1,
        /// <summary>
        /// 伤害判定
        /// </summary>
        Damage = 0x2,
        QTE = 0x4,
        /// <summary>
        /// 自身动作产生的后摇，可以通过一些动作取消
        /// </summary>
        HouYao = 0x8,
        /// <summary>
        /// 硬直， 受到攻击时会硬直
        /// </summary>
        YingZhi = 0x10,

        InputBusy = 0x20,

        AttackBusy = 0x40,


    }


    public enum ActType {
        None,
        Move,
        Attack,
        Dodge,
        Burst,
        BeAttack
    }

    public enum RangeType {
        Circle,
        Rect,
        Sector,
    }
    public class Act {
        public tAct table;
        public Act[] next;

        public int level;

        public AnimationClip anim;
        
        public bool Enabled => level > 0;

        public Act(int id) {
            table = Table<tAct>.Find(id);
            Log.ErrorIf(table == null, $"{id} is not in ActTable");
            if (table.next != null) {
                var len = table.next.Length;
                next = new Act[len];
                for(var i = 0; i < len; ++i) {
                    next[i] = new Act(table.next[i]);
                }
            }
            level = table.type == (int)ActType.Attack ? 1 : 0;
            //AssetMgr.Instance.LoadAsync(table.anim, (asset) => { anim = asset as AnimationClip; });
        }

        public void AttackJudge(Role role) {
            var range = table.range;
            var rangeType = (RangeType)range[0];
            List<Role> roles = null;
            switch (rangeType) {
            case RangeType.Rect:
                var w = range[1];
                var h = range[2];
                var rect = new Rect(-w / 2f, 0, w, h);
                roles = EntityManager.Instance.FindRolesInRangeRect(role, rect);
                break;
            case RangeType.Sector:
                var r = range[1];
                var cosThetaR = Mathf.Cos(range[2]) * r;
                roles = EntityManager.Instance.FindRolesInRangeSector(role, r, cosThetaR);
                break;
            case RangeType.Circle:
                roles = EntityManager.Instance.FindRolesInRangeCircle(role, range[1]);
                break;
            default:
                Log.Error($"unknown range {range[0]}");
                return;
            }
            foreach(var target in roles) {
                ApplyAttack(role, target);
            }
        }

        private void ApplyAttack(Role role, Role target) {
            var dmg = 100f;
            target.fightProps.hp -= dmg * table.dmgRate;

            //target

        }
    }


    public class ActController {

        public Role role;

        public ActController(Role role) {
            this.role = role;
            InitActCombos();
        }

        #region 状态
        public uint mStatus;

        public bool IsStatus(EActStatus s) {
            return (mStatus & (uint)s) != 0;
        }

        public void SetStatus(uint s) {
            mStatus |= (uint)s;
        }

        public void ResetStatus(uint s) {
            mStatus &= ~(uint)s;
        }

        
        #endregion
        #region ACT
        public Act[] actCombos;

        public Act curAct;

        public void InitActCombos() {
            var ids = role.table.actIds;
            actCombos = new Act[ids.Length];
            for (var i = 0; i < ids.Length; ++i) {
                actCombos[i] = new Act(ids[i]);
            }
        }

        public void PlayAct(ActType actType) {
            Act act = null;
            if (curAct != null && curAct.next != null) {
                foreach (var a in curAct.next) {
                    if (a.table.type == (int)actType) {
                        act = a;
                        break;
                    }
                }
            }
            if (act == null) {
                foreach (var a in actCombos) {
                    if (a.table.type == (int)actType) {
                        act = a;
                        break;
                    }
                }
            }
            Log.ErrorIf(act == null, $"Not Found Act {actType}");
            curAct = act;
            if (role.move.IsMoving) {
                role.move.StopMove();
            }
            Log.ErrorIf(act.anim == null, $"Not Found Act anim {act.table.anim},{act}");
            role.avatar.AnimCtrl.PlayAnimation(act.anim, null);
        }

        public void OnActJudge(int p) {


        }
        #endregion


        //public bool Spell(int skillId) {
        //    if (IsStatus(EActStatus.AttackBusy)) {
        //        return false;
        //    }
        //    var skill = role.GetSkill(skillId);
        //    if (skill.IsCd()) {
        //        return false;
        //    }
        //    skill.Play();
        //    return true;
        //}

        public ActType cacheAct;
        public float cacheActTime;

        public void ClearCache() {
            cacheAct = ActType.None;
        }
        public void Attack() {
            var state = (uint)EActStatus.AttackBusy | (uint)EActStatus.YingZhi;
            if ((mStatus & state) != 0) {
                cacheAct = ActType.Attack;
                cacheActTime = GameTime.time;
                return;
            }
            cacheAct = ActType.None;
            PlayAct(ActType.Attack);
        }

        public void Move(Vector2 offset) {
            var state = (uint)EActStatus.AttackBusy | (uint)EActStatus.YingZhi;
            if ((mStatus & state) != 0) {
                return;
            }
            role.move.Move(offset);
        }

        /// <summary>
        /// 闪避
        /// </summary>
        public void Dodge() {

        }

        public void BeAttack(Role role) {
            var state = (uint)EActStatus.Stoic;
            if ((mStatus & state) == 0) {
                PlayAct(ActType.BeAttack);
            }
        }
    }
}
