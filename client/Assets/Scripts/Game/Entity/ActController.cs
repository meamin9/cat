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
        Burst
    }

    public class Act {
        public ActTable table;
        public Act[] next;

        public int level;

        public AnimationClip anim;
        
        public bool Enabled => level > 0;

        public Act(int id) {
            table = Table<ActTable>.Find(id);
            Log.ErrorIf(table == null, $"{id} is not in ActTable");
            if (table.next != null) {
                var len = table.next.Length;
                next = new Act[len];
                for(var i = 0; i < len; ++i) {
                    next[i] = new Act(table.next[i]);
                }
            }
            level = table.type == (int)ActType.Attack ? 1 : 0;
        }
    }


    public class ActController {

        public Role role;

        public ActController(Role role) {
            this.role = role;
        }

        #region 状态
        public uint mStatus;

        public bool IsStatus(EActStatus s) {
            return (mStatus & (uint)s) != 0;
        }

        public bool OnNormalAttackInput() {
            var state = (uint)EActStatus.InputBusy | (uint)EActStatus.YingZhi;
            if ((mStatus & state) != 0) {
                return false;
            }
            return true;
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

        public void InitActCombos(Act act) {
            var ids = role.table.actIds;
            actCombos = new Act[ids.Length];
            for (var i = 0; i < ids.Length; ++i) {
                actCombos[i] = new Act(ids[i]);
            }
        }

        public bool CheckAct(ActType actType) {
            return true;
        }

        public void PlayAct(ActType actType) {
            Act act = null;
            if (curAct != null) {
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
            role.avatar.AnimCtrl.PlayAnimation(act.anim, null);
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
            PlayAct(ActType.Move);
        }

        public void Stand() {
            var state = (uint)EActStatus.AttackBusy | (uint)EActStatus.YingZhi;
            if ((mStatus & state) != 0) {
                return;
            }
        }

        /// <summary>
        /// 闪避
        /// </summary>
        public void Dodge() {

        }
    }
}
