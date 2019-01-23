using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 提示模块：包括飘字、跑马灯、提示框、交互弹窗
 * 1. 对于连续多个提示，能够全部显示出来
 */


class NoticeModel
{
    //private 
    public static void RegisterProto(string content)
    {
        //Net.Instance.RegisterProto<proto.SCNotice>((msg, ses) => {
        //    ShowText(Lang.Instance.Notice((NoticeKey)(msg.Index)));
        //});
    }

    // 显示一条弹窗文字提示
    public static void ShowText(string content)
    {
        Debug.Log(content);
        //Text text = new Text();
    }

}