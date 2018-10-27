using UnityEngine;

/// <summary>
/// 悬崖UI提示.
/// </summary>
public class SSXuanYaTiShi : MonoBehaviour
{
    static SSXuanYaTiShi _Instance;
    public static SSXuanYaTiShi GetInstance()
    {
        return _Instance;
    }

    // Use this for initialization
    void Start ()
    {
        _Instance = this;
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    bool IsRemoveSelf = false;
    public void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}
