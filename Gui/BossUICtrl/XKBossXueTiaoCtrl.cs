﻿using UnityEngine;
using System.Collections;

public class XKBossXueTiaoCtrl : MonoBehaviour
{
	public UISprite BossXueTiaoSprite;
	public UISprite BossXueTiaoHongSprite;
	/**
	 * 填充血条的速度.
	 */
	[Range(0.001f, 10f)]public float SpeedFillXueTiao = 0.5f;
	/**
	 * 减少红色血条的速度.
	 */
	[Range(0.001f, 10f)]public float SpeedSubXueTiao = 0.1f;
	bool IsFillBossXueTiao;
	public static bool IsWuDiPlayer;
	static XKBossXueTiaoCtrl _Instance;
	public static XKBossXueTiaoCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start ()
	{
		_Instance = this;
		IsWuDiPlayer = false;
		HiddenBossXueTiao();
	}

	void Update()
	{
		if (IsFillBossXueTiao) {
			FillBossXueTiaoSprite();
		}

		if (IsCanSubXueTiaoAmount) {
			SubBossXueTiaoHongSprite();
		}
	}

	XKNpcHealthCtrl BossHealthScript;
	public void SetBloodBossAmount(float bloodAmount, XKNpcHealthCtrl bossHealth = null)
	{
		if (bossHealth != null && bloodAmount == -1f) {
			BossHealthScript = bossHealth; //存储Boss血量脚本.
		}

		if (!IsCanSubXueTiaoAmount) {
			return;
		}

		if (!gameObject.activeSelf) {
			return;
		}

		bloodAmount = bloodAmount > 1f ? 1f : bloodAmount;
		bloodAmount = bloodAmount < 0f ? 0f : bloodAmount;
		BossXueTiaoSprite.fillAmount = bloodAmount;
		if (bloodAmount <= 0f) {
			//JiFenJieMianCtrl.GetInstance().ShowFinishTaskInfo();
			IsWuDiPlayer = true;
			Invoke("DelayActiveJiFenJieMian", 4f);
			XkGameCtrl.BossRemoveAllNpcAmmo();
			AudioBeiJingCtrl.StopGameBeiJingAudio();
			HiddenBossXueTiao();
			if (BossHealthScript != null && bossHealth == null) {
				BossHealthScript.OnDamageNpc(99999999, PlayerEnum.Null);
			}
		}
	}

	void DelayActiveJiFenJieMian()
	{
		if (GameTimeBossCtrl.GetInstance().GetTimeBossResidual() > 0) {
			XKGlobalData.GetInstance().PlayAudioBossShengLi();
		}
		JiFenJieMianCtrl.GetInstance().ActiveJiFenJieMian();
	}

	public void HiddenBossXueTiao()
	{
        if (BossZuDangCtrl.GetInstance() != null)
        {
            BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(false);
        }
		gameObject.SetActive(false);
		GameTimeBossCtrl.GetInstance().HiddenGameTime();
	}

	public void OpenBossXueTiao(int timeVal = 180)
	{
		//timeVal = 150;
		//timeVal = 10; //test.
		IsCanSubXueTiaoAmount = false;
		BossXueTiaoHongSprite.fillAmount = 0f;
		BossXueTiaoSprite.fillAmount = 0f;
		BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(true);
		XKTriggerStopMovePlayer.IsActiveTrigger = true;
		GameTimeCtrl.GetInstance().HiddenGameTime();
		//BossXueTiaoSprite.fillAmount = 1f;
		gameObject.SetActive(true);
		TweenAlpha TwAlpha = gameObject.AddComponent<TweenAlpha>();
		TwAlpha.from = 0f;
		TwAlpha.to = 1f;
		TwAlpha.duration = 0.1f;
		EventDelegate.Add(TwAlpha.onFinished, delegate{
			ChangeBossXTAlphaEnd(timeVal);
		});
		TwAlpha.PlayForward();
	}

	void ChangeBossXTAlphaEnd(int timeVal)
	{
		GameTimeBossCtrl.GetInstance().ActiveIsCheckTimeSprite(timeVal);
		StartFillBossXueTiao();
	}

	void StartFillBossXueTiao()
	{
		IsFillBossXueTiao = true;
	}
	
	void CloseFillBossXueTiao()
	{
		IsFillBossXueTiao = false;
		IsCanSubXueTiaoAmount = true;
	}

	bool IsCanSubXueTiaoAmount;
	public bool GetIsCanSubXueTiaoAmount()
	{
		return IsCanSubXueTiaoAmount;
	}

	void FillBossXueTiaoSprite()
	{
		Vector2 startVec = new Vector2(BossXueTiaoHongSprite.fillAmount, BossXueTiaoSprite.fillAmount);
		float addAmount = Time.deltaTime * SpeedFillXueTiao;
		startVec += new Vector2(addAmount, addAmount);
		startVec.x = startVec.x > 1f ? 1f : startVec.x;
		startVec.y = startVec.y > 1f ? 1f : startVec.y;
		if (startVec.y >= 1f) {
			BossXueTiaoHongSprite.fillAmount = 1f;
			BossXueTiaoSprite.fillAmount = 1f;
			CloseFillBossXueTiao();
			return;
		}

		//Debug.Log("Unity:"+"startVec "+startVec);
		BossXueTiaoHongSprite.fillAmount = startVec.x;
		BossXueTiaoSprite.fillAmount = startVec.y;
	}

	void SubBossXueTiaoHongSprite()
	{
		if (BossXueTiaoHongSprite.fillAmount <= BossXueTiaoSprite.fillAmount) {
			return;
		}

		float fillAmount = BossXueTiaoHongSprite.fillAmount - (Time.deltaTime * SpeedSubXueTiao);
		if (fillAmount < BossXueTiaoSprite.fillAmount) {
			fillAmount = BossXueTiaoSprite.fillAmount;
		}
		BossXueTiaoHongSprite.fillAmount = fillAmount;
	}
}