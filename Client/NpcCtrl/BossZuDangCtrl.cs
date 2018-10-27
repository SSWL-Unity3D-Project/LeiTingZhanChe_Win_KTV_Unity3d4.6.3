using UnityEngine;
using System.Collections;

public class BossZuDangCtrl : MonoBehaviour
{
	static BossZuDangCtrl _Instance;
	public static BossZuDangCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Awake()
	{
		_Instance = this;
        BoxCollider boxCol = gameObject.GetComponent<BoxCollider>();
        boxCol.gameObject.layer = LayerMask.NameToLayer("UI");
        boxCol.renderer.enabled = false;

        //BoxCollider[] boxColArray = gameObject.GetComponentsInChildren<BoxCollider>();
		//foreach (BoxCollider item in boxColArray) {
		//	item.gameObject.layer = LayerMask.NameToLayer("UI");
		//	item.renderer.enabled = false;
		//}
		SetIsActiveBossZuDang(false);
	}

	public void SetIsActiveBossZuDang(bool isActive)
	{
		gameObject.SetActive(isActive);
	}
}