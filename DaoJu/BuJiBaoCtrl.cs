using UnityEngine;
using System.Collections;

public enum BuJiBaoType
{
	Null,
	DaoDan,			//导弹.
	GaoBaoDan,		//高爆弹.
	SanDan,			//散弹.
	GenZongDan,		//跟踪弹.
	ChuanTouDan,	//穿透弹（穿甲弹）.
	JianSuDan,		//减速弹.
	NLHuDun,		//能量护盾.
	FenShuDJ,				//分数道具.
	JiSuDJ,					//急速道具.
	YiLiaoBaoDJ,			//医疗包道具.
	ShuangBeiFenShuDJ,		//加倍分数道具.
	QianHouFireDJ,			//前后发射道具.
	ChangChengJiQiang,		//长程机枪道具.
	SanDanJiQiang,			//散弹机枪道具.
	QiangJiJiQiang,			//强击机枪道具.
	PaiJiPaoDJ,				//迫击炮道具.
	ZhuPaoSanDanDJ,			//主炮散弹道具.
	HuoLiAllOpenDJ,			//主炮和机枪火力全开道具.
}

public enum PlayerEnum
{
	Null,
	PlayerOne,
	PlayerTwo,
	PlayerThree,
	PlayerFour,
}

public class BuJiBaoCtrl : MonoBehaviour {
	public bool IsOpenCiLi = true;
	public BuJiBaoType BuJiBao;
	public Animator AniCom;
	public GameObject ExplodeObj;
	[Range(0.1f, 30f)]public float DestroyTime = 2f;
	/**
	 * 补给包高度计算斜率.
	 */
	[Range(0f, 1000f)]public float BuJiBaoGDKey = 0.5f;
	[Range(0.01f, 10f)]public float DaoJuFlyTime = 0.5f;
	public GameObject DaoJuCore;
	[Range(0, 10000)]public int FenShuVal = 100;
	[Range(2, 18)]public int FenShuBeiLv = 2;
	bool IsDeath;
	bool IsDelayDestroy;
	bool IsSpawnDaoJu;
	bool IsMoveOverDaoJuByItween;
	float TimeCheckDis;
	Transform AimPlayerTr;
	Transform DaoJuTr;
	bool IsMoveOverDaoJuToPlayer;
	BoxCollider BoxCol;
//	NetworkView NetworkViewCom;
	void Start()
	{
		DaoJuTr = transform;
		BoxCol = GetComponent<BoxCollider>();
	}

	void Update()
	{
		CheckCameraDis();

		if (IsHiddenDaoJuTr) {
			return;
		}

		if (!IsOpenCiLi) {
			return;
		}
		CheckPlayerDistance();
		MoveDaoJuToPlayer();
	}

	float TimeLastCamDis = 0f;
	bool IsHiddenDaoJuTr;
	static Transform CamTr;
	void CheckCameraDis()
	{
		if (Time.time - TimeLastCamDis < 1f) {
			return;
		}
		TimeLastCamDis = Time.time;

		if (CamTr == null) {
			CamTr = Camera.main == null ? null : Camera.main.transform;
			return;
		}

		bool isHiddenDaoJu = false;
		Vector3 posA = DaoJuTr.position;
		Vector3 posB = CamTr.position;
		posA.y = posB.y = 0f;
		float disVal = Vector3.Distance(posA, posB);

		Vector3 vecBA = posA - posB;
		Vector3 vecCam = CamTr.forward;
		vecBA.y = vecCam.y = 0f;

		if (disVal > 100f || Vector3.Dot(vecBA, vecCam) < 0f) {
			isHiddenDaoJu = true;
		}

		IsHiddenDaoJuTr = isHiddenDaoJu;
		Transform childTr = DaoJuTr.GetChild(0);
		childTr.gameObject.SetActive(!isHiddenDaoJu);
	}

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("Unity:"+"OnCollisionEnter -> nameHit "+collision.gameObject.name);
		string layerName = LayerMask.LayerToName(collision.gameObject.layer);
		if (layerName == XkGameCtrl.TerrainLayer
		    && IsSpawnDaoJu
		    && !IsDelayDestroy) {
			InitDelayDestroyBuJiBao();
		}
		
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		XKPlayerMoveCtrl script = collision.transform.root.GetComponent<XKPlayerMoveCtrl>();
		if (script == null) {
			return;
		}
		RemoveBuJiBao(script.PlayerIndex);
	}

	void InitDelayDestroyBuJiBao()
	{	
		if (IsDelayDestroy) {
			return;
		}
		IsDelayDestroy = true;
		if (AniCom != null) {
			AniCom.SetBool("LuoDi", true);
		}
		
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				return;
			}
		}
		Invoke("DelayDestroyBuJiBao", DestroyTime);
	}

	void DelayDestroyBuJiBao()
	{
		RemoveBuJiBao(PlayerEnum.Null);
	}

	/// <summary>
	/// Removes the bu ji bao. playerSt == 0 -> hit TerrainLayer,
	/// playerSt == 1 -> PlayerOne, playerSt == 2 -> PlayerTwo.
	/// playerSt == 3 -> PlayerThree, playerSt == 4 -> PlayerFour.
	/// </summary>
	/// <param name="key">Key.</param>
	public void RemoveBuJiBao(PlayerEnum playerSt, int keyHit = 0)
	{
		if (IsDeath) {
			return;
		}
		IsDeath = true;
		CancelInvoke("DelayDestroyBuJiBao");
		if (playerSt != PlayerEnum.Null || keyHit == 1) {
			//XKGlobalData.GetInstance().PlayAudioHitBuJiBao();
			if (ExplodeObj != null) {
				GameObject obj = (GameObject)Instantiate(ExplodeObj, transform.position, transform.rotation);
				XkGameCtrl.CheckObjDestroyThisTimed(obj);
			}
			
			if (Network.peerType != NetworkPeerType.Server) {
				bool isMoveDaoJu = true;
				switch (BuJiBao) {
				case BuJiBaoType.FenShuDJ:
					isMoveDaoJu = false;
					XKPlayerFenShuCtrl.GetInstance().ShowPlayerFenShu(playerSt, FenShuVal);
					break;
				case BuJiBaoType.JiSuDJ:
					//isMoveDaoJu = false;
					XKPlayerMoveCtrl.SetPlayerJiSuMoveSpeed(playerSt);
					XKPlayerMoveCtrl.SetPlayerJiSuState(playerSt);
					XKPlayerJiSuCtrl.GetInstance().ShowPlayerJiSu(playerSt);
					break;
				case BuJiBaoType.YiLiaoBaoDJ:
					isMoveDaoJu = false;
					XkGameCtrl.AddPlayerHealth(playerSt, XKDaoJuGlobalDt.GetInstance().YiLiaoBaoXueLiangVal);
					XKPlayerJiJiuBaoCtrl.GetInstance().ShowPlayerJiJiuBao(playerSt);
					break;
				case BuJiBaoType.ShuangBeiFenShuDJ:
					//isMoveDaoJu = false;
					//XKDaoJuGlobalDt.SetTimeFenShuBeiLv(playerSt, FenShuBeiLv);
					XKDaoJuGlobalDt.SetTimeFenShuBeiLv(playerSt, 2);
					XKFenShuBeiLvCtrl.GetInstance().ShowPlayerFenShuBeiLv(playerSt);
					break;
				case BuJiBaoType.QianHouFireDJ:
					isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerQianHouFire(playerSt);
					break;
				case BuJiBaoType.ChangChengJiQiang:
					isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerChangChengFire(playerSt);
					break;
				case BuJiBaoType.SanDanJiQiang:
					isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerJiQiangSanDanFire(playerSt);
					break;
				case BuJiBaoType.QiangJiJiQiang:
					isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerQiangJiFire(playerSt);
					break;
				case BuJiBaoType.PaiJiPaoDJ:
					isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerIsPaiJiPaoFire(playerSt);
					break;
				case BuJiBaoType.ZhuPaoSanDanDJ:
					isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerIsSanDanZPFire(playerSt);
					break;
				case BuJiBaoType.HuoLiAllOpenDJ:
					//isMoveDaoJu = false;
					XKDaoJuGlobalDt.SetPlayerIsHuoLiAllOpen(playerSt);
					XKPlayerHuoLiAllOpenCtrl.GetInstance().ShowPlayerHuoLiOpen(playerSt);
					break;
				case BuJiBaoType.ChuanTouDan:
					isMoveDaoJu = false;
					XKPlayerAutoFire.GetInstanceAutoFire(playerSt).SetAmmoStateZhuPao(PlayerAmmoType.ChuanTouAmmo);
					break;
				case BuJiBaoType.DaoDan:
				case BuJiBaoType.GaoBaoDan:
				case BuJiBaoType.SanDan:
				case BuJiBaoType.GenZongDan:
				case BuJiBaoType.JianSuDan:
				case BuJiBaoType.NLHuDun:
					isMoveDaoJu = false;
					break;
				}

				if (isMoveDaoJu) {
					DaoJuCtrl.GetInstance().MoveDaoJuObjToPlayer(playerSt, transform);
				}
			}
		}
		DestroyNetObj(gameObject);
	}

	[RPC] void BuJiBaoSendRemoveObj()
	{
		if (IsDeath) {
			return;
		}
		IsDeath = true;

		if (ExplodeObj != null) {
			GameObject obj = (GameObject)Instantiate(ExplodeObj, transform.position, transform.rotation);
			XkGameCtrl.CheckObjDestroyThisTimed(obj);
		}
		DestroyNetObj(gameObject);
	}

	void DestroyNetObj(GameObject obj)
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			Destroy(obj);
		}
		else {
			if (Network.peerType == NetworkPeerType.Server) {
				if (NetworkServerNet.GetInstance() != null) {
					NetworkServerNet.GetInstance().RemoveNetworkObj(obj);
				}
			}
		}
	}

	void SetBuJiBaoRigbody(bool isKine)
	{
		if (rigidbody == null) {
			return;
		}
		rigidbody.isKinematic = isKine;
	}

	public void MoveDaoJuToPoint(Transform trEndPoint)
	{
		SetBuJiBaoRigbody(true);
		Vector3 endPos = trEndPoint.position;
		Vector3 startPos = trEndPoint.position + Vector3.up * 2f;
		Vector3 hitForward = Vector3.down;
		//Vector3 startPos = trEndPoint.position - trEndPoint.forward * 2f; //test.
		//Vector3 hitForward = trEndPoint.forward; //test.
		RaycastHit hit;
		if (Physics.Raycast(startPos, hitForward, out hit, 50f, XkGameCtrl.GetInstance().LandLayer)) {
			endPos = hit.point + Vector3.up * 0.5f;
		}

		Vector3 posA = trEndPoint.position;
		Vector3 posB = transform.position;
		posA.y = posB.y = 0f;
		float paoDanMVDis = Vector3.Distance(posA, posB);
		float lobHeight = BuJiBaoGDKey * paoDanMVDis + 0.5f;
		float lobTime = DaoJuFlyTime;
		iTween.MoveBy(DaoJuCore, iTween.Hash("y", lobHeight,
		                                    "time", lobTime * 0.5f,
		                                    "easeType", iTween.EaseType.easeOutQuad));
		iTween.MoveBy(DaoJuCore, iTween.Hash("y", -lobHeight,
		                                    "time", lobTime * 0.5f,
		                                    "delay", lobTime * 0.5f,
		                                    "easeType", iTween.EaseType.easeInCubic));
		iTween.MoveTo(gameObject, iTween.Hash("position", endPos,
		                                   "time", lobTime,
		                                   "easeType", iTween.EaseType.linear,
		                                   "oncomplete", "MoveDaoJuOnCompelteITween"));
	}

	void MoveDaoJuOnCompelteITween()
	{
		IsMoveOverDaoJuByItween = true;
		SetBuJiBaoRigbody(false);
	}
	
	public void SetIsSpawnDaoJu()
	{
		IsSpawnDaoJu = true;
        transform.SetParent(XkGameCtrl.GetInstance().DaoJuArray);
    }

	void CheckPlayerDistance()
	{
		if (Time.realtimeSinceStartup - TimeCheckDis < 0.2f) {
			return;
		}
		TimeCheckDis = Time.realtimeSinceStartup;

		if (!IsMoveOverDaoJuByItween && IsSpawnDaoJu) {
			return;
		}

		if (AimPlayerTr != null) {
			return;
		}

		Transform playerTr = null;
		Vector3 posA = Vector3.zero;
		Vector3 posB = DaoJuTr.position;
		for (int i = 0; i < 4; i++) {
			if (XKPlayerGlobalDt.PlayerMoveList == null || XKPlayerGlobalDt.PlayerMoveList[i] == null) {
				continue;
			}

			if (XKPlayerGlobalDt.PlayerMoveList[i].GetIsDeathPlayer()) {
				continue;
			}
			playerTr = XKPlayerGlobalDt.PlayerMoveList[i].transform;
			posA = playerTr.position;
			posA.y = posB.y = 0f;
			if (Vector3.Distance(posA, posB) > XKDaoJuGlobalDt.GetInstance().CiLiDaoJuDis) {
				continue;
			}
			//Debug.Log("Unity:"+"player "+XKPlayerGlobalDt.PlayerMoveList[i].name);

			AimPlayerTr = XKPlayerGlobalDt.PlayerMoveList[i].transform;
			SetBuJiBaoRigbody(true);
			if (rigidbody != null) {
				rigidbody.useGravity = false;
			}

			if (BoxCol != null) {
				BoxCol.enabled = false;
			}
		}
	}

	void MoveDaoJuToPlayer()
	{
		if (IsMoveOverDaoJuToPlayer) {
			return;
		}

		if (AimPlayerTr == null) {
			return;
		}
		Vector3 dirVal = AimPlayerTr.position - DaoJuTr.position;
		dirVal = dirVal.normalized * XKDaoJuGlobalDt.GetInstance().CiLiDaoJuSpeed * Time.deltaTime;
		DaoJuTr.Translate(dirVal, Space.World);
		if (Vector3.Distance(AimPlayerTr.position, DaoJuTr.position) <= 0.5f) {
			//Debug.Log("Unity:"+"MoveDaoJuToPlayer...");
			IsMoveOverDaoJuToPlayer = true;
			XKPlayerMoveCtrl script = AimPlayerTr.GetComponent<XKPlayerMoveCtrl>();
			if (script == null) {
				return;
			}
			RemoveBuJiBao(script.PlayerIndex);
		}
	}
}