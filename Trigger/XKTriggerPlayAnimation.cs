﻿using UnityEngine;
using System.Collections;

public class XKTriggerPlayAnimation : MonoBehaviour {
	public Animator NpcAnimator;
	public AnimatorNameNPC AniName;

	void Start()
	{
		if (NpcAnimator == null) {
			Debug.LogError("Unity:"+"NpcAnimator is null");
			NpcAnimator.name = "null";
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}

		PlayAnimation();
        Destroy(gameObject);
    }

	public void PlayAnimation()
	{
		if (AniName == AnimatorNameNPC.Null) {
			return;
		}
		NpcAnimator.SetBool(AniName.ToString(), true);
	}
}
