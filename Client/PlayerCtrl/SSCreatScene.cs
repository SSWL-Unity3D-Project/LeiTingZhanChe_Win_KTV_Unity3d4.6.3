using UnityEngine;

public class SSCreatScene : SSGameMono
{
    public GameObject[] SceneArray;
    public void Init()
    {
        SceneArray = new GameObject[4];
        //创建第一个游戏场景.
        CreatGameScene((int)SSTriggerManageScene.SceneInfo.Scene01);
    }

    /// <summary>
    /// 创建游戏场景.
    /// </summary>
    public void CreatGameScene(int index)
    {
        if (index >= 0 && index < SceneArray.Length)
        {
            if (SceneArray[index] == null)
            {
                UnityLog("CreatGameScene -> index ================ " + index);
                int indesScene = index + 1;
                string prefabInfo = "Prefabs/Scene/Scene0" + indesScene.ToString();
                GameObject gmDataPrefab = (GameObject)Resources.Load(prefabInfo);
                if (gmDataPrefab != null)
                {
                    SceneArray[index] = (GameObject)Instantiate(gmDataPrefab, XkGameCtrl.MissionCleanup);
                }
                else
                {
                    UnityLogWarning("CreatGameScene -> gmDataPrefab was null!");
                    UnityLogWarning("CreatGameScene -> prefabInfo == " + prefabInfo);
                }
            }
        }
    }
    
    /// <summary>
    /// 删除游戏场景.
    /// </summary>
    public void RemoveGameScene(int index)
    {
        UnityLog("RemoveGameScene -> index ================ " + index);
        if (index >= 0 && index < SceneArray.Length)
        {
            if (SceneArray[index] != null)
            {
                Destroy(SceneArray[index]);
                SceneArray[index] = null;
            }
        }
    }
}
