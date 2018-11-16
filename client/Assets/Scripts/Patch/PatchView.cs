using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Base;
using UnityEngine.UI;

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
            _slider = gameObject.transform.Find("Progress").GetComponent<Slider>();
            _bg = gameObject.transform.Find("Bg").GetComponent<RawImage>();


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

        private RawImage _bg;
        private Text _tip;
        private Slider _slider;

    }
}
