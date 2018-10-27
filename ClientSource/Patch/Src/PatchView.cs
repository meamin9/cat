using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Base;

namespace Automata.Patch
{
    public class PatchUI : IBaseUI
    {
        public static int Index { get { return 1; } }
        public int Id { get { return Index; } }
        public string PrefabPath { get { return "View/PatchUI.prefab"; } }
        public GameObject gameObject { get; set; }
        public void OnAttach()
        {

        }
        public void OnDetach() { }
        public void OnShow() { }
        public void OnHide() { }

        //public GameObject gameObject { get; set; }
        //public MonoAdaptor Adaptor;
        //public void OnAttach()
        //{
        //    Adaptor = gameObject.GetComponent<MonoAdaptor>();

        //}
    }
}
