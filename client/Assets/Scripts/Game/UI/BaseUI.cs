using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base;

namespace Game
{
    public interface IBaseUI
    {
        UIType UIType { get; set; }
        string PrefabPath { get; }
        GameObject gameObject { get; set; }
        void OnAttach();
        void OnDetach();
        void OnShow();
        void OnHide();

    }
    public class BaseUI: IBaseUI
    {
        public UIType UIType { get; set; }
        public string PrefabPath { get; set; }
        public GameObject gameObject { get; set; }
        public virtual void OnAttach()
        {

        }
        public virtual void OnDetach()
        {

        }
        public virtual void OnShow()
        {
            Log.Info("on show: {0}", UIType);
        }
        public virtual void OnHide()
        {
            Log.Info("on hide: {0}", UIType);
        }

        public void Close()
        {
            UIManager.Close(UIType);
        }
    }
}
