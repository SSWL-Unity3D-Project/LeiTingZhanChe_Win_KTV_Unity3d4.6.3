using UnityEngine;

public class SSUIRoot : SSGameMono
{
    static SSUIRoot _Instance;
    public static SSUIRoot GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_SSUIRoot");
            _Instance = obj.AddComponent<SSUIRoot>();
        }
        return _Instance;
    }

    /// <summary>
    /// 游戏UI管理组件.
    /// </summary>
    internal SSGameUICtrl m_GameUIManage;
    /// <summary>
    /// 退出游戏UI界面控制脚本.
    /// </summary>
    [HideInInspector]
    public SSExitGameUI m_ExitUICom;

}