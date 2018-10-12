using UnityEngine;
using System.Collections;

public class SSGameUICtrl : SSGameMono
{
    /// <summary>
    /// 游戏UI界面中心锚点.
    /// </summary>
    public Transform m_GameUICenter;
    /// <summary>
    /// 是否产生网络故障UI界面.
    /// </summary>
    bool IsCreatWangLuoGuZhang = false;
    /// <summary>
    /// 产生网络故障UI界面.
    /// </summary>
    internal void CreatWangLuoGuZhangUI()
    {
        Debug.Log("Unity: CreatWangLuoGuZhangUI...");
        if (IsCreatWangLuoGuZhang == false)
        {
            IsCreatWangLuoGuZhang = true;
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/wangLuoGuZhang/WangLuoGuZhang");
            if (gmDataPrefab != null)
            {
                Instantiate(gmDataPrefab, m_GameUICenter);
            }
            else
            {
                UnityLogWarning("CreatWangLuoGuZhangUI -> gmDataPrefab was null!");
            }
        }
    }

    /// <summary>
    /// 是否产生修改系统时间UI.
    /// </summary>
    bool IsCreatFixSystemTime = false;
    /// <summary>
    /// 创建修改系统时间UI提示.
    /// </summary>
    internal void CreatFixSystemTimeUI()
    {
        if (IsCreatFixSystemTime == false)
        {
            IsCreatFixSystemTime = true;
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/FixSystemTime/FixTime");
            if (gmDataPrefab != null)
            {
                Instantiate(gmDataPrefab, m_GameUICenter);
            }
            else
            {
                UnityLogWarning("CreatFixSystemTimeUI -> gmDataPrefab was null!");
            }
        }
    }
}
