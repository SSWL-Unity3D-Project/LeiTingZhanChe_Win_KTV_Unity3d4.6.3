using UnityEngine;

public class SSMovieSceneRoot : SSGameMono
{
    bool IsCreatSceneMovie = false;
    // Use this for initialization
    void Start()
    {
        CreatGameMovieScene();
    }

    /// <summary>
    /// 创建游戏场景.
    /// </summary>
    public void CreatGameMovieScene()
    {
        Resources.UnloadUnusedAssets();
        if (IsCreatSceneMovie == false)
        {
            IsCreatSceneMovie = true;
            string prefabInfo = "Prefabs/Scene/MovieRoot";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabInfo);
            if (gmDataPrefab != null)
            {
                Instantiate(gmDataPrefab);
            }
            else
            {
                UnityLogWarning("CreatGameMovieScene -> gmDataPrefab was null!");
                UnityLogWarning("CreatGameMovieScene -> prefabInfo == " + prefabInfo);
            }
        }
    }

    //public void RemoveGameMovieScene()
    //{
    //    if (SceneMovie != null)
    //    {
    //        Destroy(SceneMovie);
    //        SceneMovie = null;
    //        Resources.UnloadUnusedAssets();
    //    }
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.P))
    //    {
    //        RemoveGameMovieScene();
    //    }
    //}
}
