﻿using UnityEngine;
using System.Collections;

public class XKTriggerClosePlayerUI : MonoBehaviour {
	public AiMark FeiJiMarkCom;
	public AiMark TanKeMarkCom;
	public static bool IsClosePlayerUI;
	public static bool IsActiveHeTiCloseUI;
	public AiPathCtrl TestPlayerPath;
	static XKTriggerClosePlayerUI _Instance;
	public static XKTriggerClosePlayerUI GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		if (Network.peerType != NetworkPeerType.Disconnected
		    && _Instance == null
		    && FeiJiMarkCom != null
		    && TanKeMarkCom != null) {
			_Instance = this;
		}
		IsActiveHeTiCloseUI = false;
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
#endif

    void OnTriggerEnter(Collider other)
	{
		if (XKTriggerKaQiuShaFire.IsCloseKaQiuShaTest) {
			return; //test;
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Server) {
				if (FeiJiMarkCom != null && TanKeMarkCom != null) {
					IsActiveHeTiCloseUI = true;
					ServerPortCameraCtrl.CloseAllServerPortCamera();
				}
				return;
			}
		}

//		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi() || GameOverCtrl.IsShowGameOver) {
//			return;
//		}

		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}

		if (playerScript.PlayerSt == PlayerTypeEnum.FeiJi && XkGameCtrl.GameJiTaiSt != GameJiTaiType.FeiJiJiTai) {
			return;
		}
		
		if (playerScript.PlayerSt == PlayerTypeEnum.TanKe && XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
			return;
		}

		if (FeiJiMarkCom != null && TanKeMarkCom != null && IsActiveHeTiCloseUI) {
			return;
		}
		IsClosePlayerUI = true;
		CheckIsPlayDaoJiShi();

		ScreenDanHeiCtrl.GetInstance().ClosePlayerUI();
		if (FeiJiMarkCom != null && TanKeMarkCom != null) {
			IsActiveHeTiCloseUI = true;
			if (Network.peerType == NetworkPeerType.Client && NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().TryActiveHeTiPlayerEvent();
			}
			ScreenDanHeiCtrl.GetInstance().OpenStartCamera();
			ScreenDanHeiCtrl.GetInstance().OpenScreenDanHui(1);
			GameMode modeVal = XkGameCtrl.GameModeVal;
			switch (modeVal) {
			case GameMode.DanJiFeiJi:
				playerScript.MakePlayerMoveToAiMark(FeiJiMarkCom);
				break;
				
			case GameMode.DanJiTanKe:
				playerScript.MakePlayerMoveToAiMark(TanKeMarkCom);
				break;
				
			case GameMode.LianJi:
				if (Network.peerType != NetworkPeerType.Disconnected) {
					if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
						XkPlayerCtrl.GetInstanceFeiJi().MakePlayerMoveToAiMark(FeiJiMarkCom);
					}

					if (XkPlayerCtrl.GetInstanceTanKe() != null) {
						XkPlayerCtrl.GetInstanceTanKe().MakePlayerMoveToAiMark(TanKeMarkCom);
					}
				}
				else {
					XkPlayerCtrl.GetInstanceFeiJi().MakePlayerMoveToAiMark(FeiJiMarkCom);
					XkPlayerCtrl.GetInstanceTanKe().MakePlayerMoveToAiMark(TanKeMarkCom);
				}
				
//				if (XkGameCtrl.GetInstance().IsServerCameraTest) {
//					ServerPortCameraCtrl.CloseAllServerPortCamera();
//				}
				break;
			}
		}
		gameObject.SetActive(false);
	}

	public void HandlePlayerOnTriggerEnter(XkPlayerCtrl playerScript)
	{
		Debug.Log("Unity:"+"HandlePlayerOnTriggerEnter...");
		if (XKTriggerKaQiuShaFire.IsCloseKaQiuShaTest) {
			return; //test;
		}
		
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Server) {
				if (FeiJiMarkCom != null && TanKeMarkCom != null) {
					IsActiveHeTiCloseUI = true;
					ServerPortCameraCtrl.CloseAllServerPortCamera();
				}
				return;
			}
		}
		
//		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi() || GameOverCtrl.IsShowGameOver) {
//			return;
//		}

		if (playerScript == null) {
			return;
		}
		
		if (playerScript.PlayerSt == PlayerTypeEnum.FeiJi && XkGameCtrl.GameJiTaiSt != GameJiTaiType.FeiJiJiTai) {
			return;
		}
		
		if (playerScript.PlayerSt == PlayerTypeEnum.TanKe && XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
			return;
		}
		
		if (FeiJiMarkCom != null && TanKeMarkCom != null && IsActiveHeTiCloseUI) {
			return;
		}
		IsClosePlayerUI = true;
		CheckIsPlayDaoJiShi();
		
		ScreenDanHeiCtrl.GetInstance().ClosePlayerUI();
		if (FeiJiMarkCom != null && TanKeMarkCom != null) {
			IsActiveHeTiCloseUI = true;
			ScreenDanHeiCtrl.GetInstance().OpenStartCamera();
			ScreenDanHeiCtrl.GetInstance().OpenScreenDanHui(1);
			GameMode modeVal = XkGameCtrl.GameModeVal;
			switch (modeVal) {
			case GameMode.DanJiFeiJi:
				playerScript.MakePlayerMoveToAiMark(FeiJiMarkCom);
				break;
				
			case GameMode.DanJiTanKe:
				playerScript.MakePlayerMoveToAiMark(TanKeMarkCom);
				break;
				
			case GameMode.LianJi:
				if (Network.peerType != NetworkPeerType.Disconnected) {
					if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
						XkPlayerCtrl.GetInstanceFeiJi().MakePlayerMoveToAiMark(FeiJiMarkCom);
					}
					
					if (XkPlayerCtrl.GetInstanceTanKe() != null) {
						XkPlayerCtrl.GetInstanceTanKe().MakePlayerMoveToAiMark(TanKeMarkCom);
					}
				}
				else {
					XkPlayerCtrl.GetInstanceFeiJi().MakePlayerMoveToAiMark(FeiJiMarkCom);
					XkPlayerCtrl.GetInstanceTanKe().MakePlayerMoveToAiMark(TanKeMarkCom);
				}
				
//				if (XkGameCtrl.GetInstance().IsServerCameraTest) {
//					ServerPortCameraCtrl.CloseAllServerPortCamera();
//				}
				break;
			}
		}
		gameObject.SetActive(false);
	}

	public static void ResetIsClosePlayerUI()
	{
		IsClosePlayerUI = false;
		if (XkGameCtrl.GetInstance().IsCartoonShootTest || Network.peerType == NetworkPeerType.Server) {
			return;
		}
		ScreenDanHeiCtrl.GetInstance().OpenPlayerUI();
	}

	/**
	 * 防止在进入关闭UI之前,主角死亡播放倒计时,当倒计时结束后游戏会退回循环动画的bug.
	 */
	public static void CheckIsPlayDaoJiShi()
	{
//		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
//			DaoJiShiCtrl.GetInstance().StopDaoJiShi();
//			XkGameCtrl.PlayerYouLiangCur = 3f;
//			if (DaoJiShiCtrl.IsActivePlayerOne) {
//				XkGameCtrl.SetActivePlayerOne(true);
//				DaoJiShiCtrl.IsActivePlayerOne = false;
//			}
//			
//			if (DaoJiShiCtrl.IsActivePlayerTwo) {
//				XkGameCtrl.SetActivePlayerTwo(true);
//				DaoJiShiCtrl.IsActivePlayerTwo = false;
//			}
//		}
	}
}