using System;

public enum EActStatus
{
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


} 
public class ActController
{
    public ActController() {
    }
}
