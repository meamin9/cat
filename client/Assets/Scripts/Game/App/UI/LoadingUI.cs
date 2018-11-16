using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Automata.Base;

namespace Automata.Game
{
    public class LoadingUI : IBaseUI
    {
        public int Id { get { return (int)UIID.Loading; } }
        public string PrefabPath { get { return "UILoading.prefab"; } }
        public GameObject gameObject { get; set; }
        public void OnAttach()
        {

        }
        public void OnDetach() { }
        public void OnShow() { }
        public void OnHide() { }

        public RawImage _bg;
        public Text _tip;
        public Slider _slider;

    }

}
