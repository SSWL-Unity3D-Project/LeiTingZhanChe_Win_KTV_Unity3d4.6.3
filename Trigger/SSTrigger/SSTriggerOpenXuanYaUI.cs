using UnityEngine;

/// <summary>
/// 打开悬崖提示UI触发器.
/// </summary>
public class SSTriggerOpenXuanYaUI : MonoBehaviour {
    
    [Range(0.01f, 100f)]
    public float TimeOpen = 3f;
    float TimeLast;
    bool IsActiveTrigger;

    void Start()
    {
        if (SSXuanYaTiShi.GetInstance() != null)
        {
            SSXuanYaTiShi.GetInstance().SetActive(false);
        }

        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            Destroy(mesh);
        }

        MeshFilter meshFt = gameObject.GetComponent<MeshFilter>();
        if (meshFt != null)
        {
            Destroy(meshFt);
        }
    }

    void Update()
    {
        if (!IsActiveTrigger)
        {
            return;
        }

        if (Time.time - TimeLast < TimeOpen)
        {
            return;
        }
        if (SSXuanYaTiShi.GetInstance() != null)
        {
            SSXuanYaTiShi.GetInstance().RemoveSelf();
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
        if (playerScript == null)
        {
            return;
        }
        IsActiveTrigger = true;
        TimeLast = Time.time;
        if (SSXuanYaTiShi.GetInstance() != null)
        {
            SSXuanYaTiShi.GetInstance().SetActive(true);
        }
    }
}
