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
        public static Role Player { get; private set; }


        private Dictionary<int, Role> mRoles = new Dictionary<int, Role>();

        public Role CreateRole(int id)
        {
            var role = new Role(id);
            mRoles.Add(role.guid, role);
            return role;
        }

        #region 搜索范围内实体
        private List<Role> mRoleTempList = new List<Role>();
        public List<Role> FindRolesInRangeSector(Role role, float radius, float cosThetaR) {
            var pos3 = role.avatar.transform.position;
            var origin = new Vector2(pos3.x, pos3.z);
            var worldR3 = role.avatar.transform.TransformPoint(0f, 0f, radius);
            var centerR = new Vector2(worldR3.x, worldR3.z) - origin;
            var sqrRadius = radius * radius;
            mRoleTempList.Clear();
            var list = mRoleTempList;// new List<Role>();
            foreach(var r in mRoles.Values) {
                if (!r.CanSelected()) {
                    continue;
                }
                pos3 = r.avatar.transform.position;
                var delta = new Vector2(pos3.x, pos3.z) - origin;
                if (delta.sqrMagnitude < sqrRadius) {
                    var dot = Vector2.Dot(delta, centerR);
                    // if dot > cosThetaR*pos.magnitude, 去掉长度的计算
                    if ((dot >= 0 && cosThetaR <= 0) || //符号不同时，dot为正
                        (dot > 0) == (cosThetaR > 0) && // 符号相同时
                        (dot > 0) == (dot * dot > cosThetaR * cosThetaR * delta.sqrMagnitude)) {
                        list.Add(r);
                    }
                }
            }
            return list;
        }

        public List<Role> FindRolesInRangeRect(Role role, Rect rect) {
            var pos3 = role.avatar.transform.TransformPoint(rect.x, 0f, rect.y);
            var origin = new Vector2(pos3.x, pos3.z);
            pos3 = role.avatar.transform.TransformDirection(rect.x + rect.width, 0f, rect.y);
            var oa = new Vector2(pos3.x, pos3.z) - origin;
            pos3 = role.avatar.transform.TransformDirection(rect.x, 0f, rect.y + rect.height);
            var ob = new Vector2(pos3.x, pos3.z) - origin;
            var oaoa = Vector2.Dot(oa, oa);
            var obob = Vector2.Dot(ob, ob);

            mRoleTempList.Clear();
            var list = mRoleTempList;// new List<Role>();
            foreach (var r in mRoles.Values) {
                if (!r.CanSelected()) {
                    continue;
                }
                pos3 = r.avatar.transform.position;
                var om = new Vector2(pos3.x, pos3.z) - origin;
                var omoa = Vector2.Dot(oa, om);
                if (omoa > 0 && omoa < oaoa) {
                    var omob = Vector2.Dot(ob, om);
                    if (omob > 0 && omob < obob) {
                        list.Add(r);
                    }
                }
            }
            return list;
        }

        public List<Role> FindRolesInRangeCircle(Role role, float radius) {
            var pos3 = role.avatar.transform.position;
            var origin = new Vector2(pos3.x, pos3.z);
            var sqrRadius = radius * radius;

            mRoleTempList.Clear();
            var list = mRoleTempList;// new List<Role>();
            foreach (var r in mRoles.Values) {
                if (!r.CanSelected()) {
                    continue;
                }
                pos3 = r.avatar.transform.position;
                var delta = new Vector2(pos3.x, pos3.z) - origin;
                if (delta.sqrMagnitude < sqrRadius) {
                    list.Add(r);
                }
            }
            return list;
        }
        #endregion
    }
}
