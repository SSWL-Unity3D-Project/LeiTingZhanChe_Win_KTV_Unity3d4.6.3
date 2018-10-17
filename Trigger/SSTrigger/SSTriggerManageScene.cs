using UnityEngine;

public class SSTriggerManageScene : MonoBehaviour
{
    public enum ManageState
    {
        CREAT = 0,
        REMOVE = 1,
    }

    public enum SceneInfo
    {
        Scene01 = 0,
        Scene02 = 1,
        Scene03 = 2,
        Scene04 = 3,
    }

    [System.Serializable]
    public class ManageSceneData
    {
        public ManageState state;
        public SceneInfo scene;
    }
    public ManageSceneData SceneData;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XkPlayerCtrl>() == null)
        {
            return;
        }

        if (XkGameCtrl.GetInstance().m_CreatSceneCom != null)
        {
            switch (SceneData.state)
            {
                case ManageState.CREAT:
                    {
                        XkGameCtrl.GetInstance().m_CreatSceneCom.CreatGameScene((int)SceneData.scene);
                        break;
                    }
                case ManageState.REMOVE:
                    {
                        XkGameCtrl.GetInstance().m_CreatSceneCom.RemoveGameScene((int)SceneData.scene);
                        break;
                    }
            }
        }
        Destroy(gameObject);
    }
}
