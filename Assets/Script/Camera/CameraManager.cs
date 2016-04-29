using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {


	public CameraManager() { s_Instance = this; }
	public static CameraManager Instance { get { return s_Instance; } }
	private static CameraManager s_Instance;


	[SerializeField] CameraState m_state;
	public CameraState State
	{
		get {
			return m_state;
		}
	}

//	public float Boundary = 0.1f;
//	public float speed = 5f;

	[SerializeField] GameObject inkPrefab;
	Dictionary<int,Ink> inkDict = new Dictionary<int, Ink>(); // key (int) for finger Index; Ink for the sprite corresponse

	public Vector3 frame;
	public Vector3 frameOffset;
	public Vector3 initPos = Vector3.zero;
	public float senseIntense = 0.2f;

	[SerializeField] float EndFadeSize = 10f;
	[SerializeField] float EndFadeTime = 2f;
	[SerializeField] float EndFadeDelay = 0;
	[SerializeField] float CameraFollowFadeTime = 0.75f;
	[SerializeField] float MaxFollowTime = 8f;

	[SerializeField] float normalOtherSize = 7f;
	[SerializeField] float focusOtherSize = 4f;

	[SerializeField] float focusTolerance = 5f;

	public Vector3 OffsetFromInit{
		get {
			Vector3 tem = LogicManager.LevelManager.GetLevelObject().transform.position - initPos;
			tem.z = 0;
			return  tem;
		}
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower , OnBlow);
		EventManager.Instance.RegistersEvent(EventDefine.PetalDestory , OnPetalDestory);
		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn , OnGrowFlower);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower , OnBlow);
		EventManager.Instance.UnregistersEvent(EventDefine.PetalDestory , OnPetalDestory);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn , OnGrowFlower);
	}

	List<Petal> blowPetals = new List<Petal>();
	void OnBlow(Message msg )
	{
		
		m_state = CameraState.Disable;
		int temNum = blowPetals.Count;
		int i = 0 ;

		while ( msg.ContainMessage( "petal" + i.ToString()))
		{
			blowPetals.Add( (Petal) msg.GetMessage("petal" + i.ToString()));
			i++;
		}

		if ( blowPetals.Count > temNum )
			CameraStartFollow( blowPetals[temNum].transform );
	}

	void OnGrowFlower( Message msg )
	{
		Petal p = msg.sender as Petal;
		if ( p != null )
		{
			for ( int i = 0 ; i < blowPetals.Count ; ++ i )
			{
				if ( blowPetals[i].ID == p.ID )
				{
					blowPetals.Clear();
					CameraStopFollow();
					break;
				}
			}
		}
	}

	void OnPetalDestory(Message msg )
	{
		if ( msg.ContainMessage( "petal" ) )
		{
			Petal p = msg.GetMessage( "petal" ) as Petal;
			for ( int i = 0 ; i < blowPetals.Count ; ++ i )
			{
				if ( blowPetals[i].ID == p.ID )
				{
					blowPetals.Clear();
					CameraStopFollow();
					break;
				}
			}
		}

	}

	[SerializeField] Transform follow;

	void CameraStartFollow(Transform trans )
	{
		if ( m_state == CameraState.Free || m_state == CameraState.Disable )
		{
			StartFollowTime = Time.time;
			m_state = CameraState.FollowTrans;
			follow = trans;
			// GetComponent<Camera>().DOOrthoSize( - CameraFollowChange , CameraFollowFadeTime ).SetRelative( true );
			Camera[] childCameras = GetComponentsInChildren<Camera>();
			foreach( Camera c in childCameras )
			{
				c.DOOrthoSize( focusOtherSize , CameraFollowFadeTime ).SetEase(Ease.InCubic);
			}
		}
	}

	void CameraStopFollow()
	{
		if ( m_state == CameraState.FollowTrans )
		{
			m_state = CameraState.Free;
			follow = null;
			// GetComponent<Camera>().DOOrthoSize( CameraFollowChange , CameraFollowFadeTime ).SetRelative( true );
			Camera[] childCameras = GetComponentsInChildren<Camera>();
			foreach( Camera c in childCameras )
			{
				c.DOOrthoSize( normalOtherSize , CameraFollowFadeTime ).SetEase(Ease.OutCubic);
			}
		}
	}

	void OnFirstFlower( Message msg )
	{
		m_state = CameraState.Free;
	}

	void OnEndLevel(Message msg )
	{
		m_state = CameraState.Disable;
		//fade This
		GetComponent<Camera>().DOOrthoSize( EndFadeSize , EndFadeTime ).SetDelay( EndFadeDelay );
		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			c.DOOrthoSize( EndFadeSize , EndFadeTime ).SetDelay( EndFadeDelay );
		}
//		transform.DOMove( Global.CAMERA_INIT_POSITION , EndFadeTime ).SetDelay(EndFadeDelay);
		LogicManager.LevelManager.GetLevelObject().transform.DOMove( Vector3.zero , EndFadeTime ).SetDelay(EndFadeDelay);
	}
		
	void Awake()
	{
		initPos.z = 0;

		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			c.orthographicSize = normalOtherSize;
		}
	}

	void Start()
	{
//		Debug.Log( " CM Start " + LogicManager.LevelManager.GetLevelObject().transform.position + "  " + initPos );
		LogicManager.LevelManager.GetLevelObject().transform.position = - initPos;
	}

	Vector3 followLastPosition = Vector3.zero;
	float StartFollowTime = 0;
	void LateUpdate () {
		UpdateFollow();
	}

	void UpdateFollow()
	{		
		
		if ( m_state == CameraState.FollowTrans )
		{
			if ( ( Time.time - StartFollowTime ) > MaxFollowTime )
			{
				CameraStopFollow();
			}


			Transform lvlTrans = LogicManager.LevelManager.GetLevelObject().transform;

			if ( follow != null )
			{
				if ( followLastPosition.magnitude > 0 )
				{
					Vector3 toPos = - follow.localPosition;
					if ( !follow.gameObject.activeSelf )
						return;
					Vector3 pos = lvlTrans.position;
					pos = Vector3.Lerp( pos , toPos , 0.1f);
					// pos = ConstrainPosition( pos );

					lvlTrans.position = pos;
				}
				followLastPosition = follow.localPosition;
			}else
			{
				followLastPosition = Vector3.zero;
			}

			if ( !ifInFrame( - lvlTrans.position ,  focusTolerance ) )
			{
				CameraStopFollow();
			}
		}
	}

	void OnDoubleTapBack( TapGesture g )
	{
		WindAdv wind = LogicManager.LevelManager.GetWind();
		wind.UISwitch();
	}
		
	void OnFingerMoveBack( FingerMotionEvent e )
	{
		if ( enabled && m_state == CameraState.Free )
		{
			bool check = false;

			switch( Application.platform )
			{
			case RuntimePlatform.Android:
			case RuntimePlatform.IPhonePlayer:
				check = FingerGestures.Touches.Count >= 2;
				break;
			default :
				check = e.Finger.Index >= 1;
				break;
			}

			if ( check )
			{
				if ( e.Phase == FingerMotionPhase.Updated )
				{
					GameObject levelObj = LogicManager.LevelManager.GetLevelObject();
					Vector3 pos = - levelObj.transform.position;
					// move the camera
					Vector3 worldDelta = - Global.V2ToV3( e.Finger.DeltaPosition ) * Global.Pixel2Unit * senseIntense;
					// pos -= Global.V2ToV3( e.Finger.DeltaPosition ) * Global.Pixel2Unit * senseIntense;
					// restrict the range
					pos = GetNewPosition( pos , worldDelta );
					levelObj.transform.position = - pos;
				}else if ( e.Phase == FingerMotionPhase.Ended )
				{
					LogicManager.LevelManager.GetWind().StartUpdateWind();
				}
			}
		}
	}

	Vector3 GetNewPosition( Vector3 _pos , Vector3 delta )
	{
		Vector3 pos = _pos + delta;
		if ( pos.x < frameOffset.x - frame.x / 2f  && delta.x < 0 )
			pos.x = _pos.x ;
		if ( pos.x > frameOffset.x + frame.x / 2f  && delta.x > 0 )
			pos.x = _pos.x ;

		if ( pos.y < frameOffset.y - frame.y / 2f  && delta.y < 0 )
			pos.y = _pos.y ;
		if ( pos.y > frameOffset.y + frame.y / 2f  && delta.y > 0 )
			pos.y = _pos.y ;

		return pos;
	}

//	Vector3 ConstrainPosition( Vector3 _pos , Vector3 delta )
//	{
//		Vector3 pos = _pos;
//
//		pos.x = Mathf.Clamp( pos.x , frameOffset.x - frame.x / 2f ,  frameOffset.x + frame.x / 2f );
//		pos.y = Mathf.Clamp( pos.y , frameOffset.y - frame.y / 2f ,  frameOffset.y + frame.y / 2f );
//		pos.z = 0;
//		return pos;
//	}

	bool ifInFrame( Vector3 pos , float tolarent )
	{
		if ( pos.x < (frameOffset.x - frame.x / 2f - tolarent) || pos.x > (frameOffset.x + frame.x / 2f + tolarent ) ) {
			return false;
		}
		if ( pos.y < (frameOffset.y - frame.y / 2f - tolarent) || pos.y > (frameOffset.y + frame.y / 2f + tolarent ) ){
			return false;
		}
		return true;
	}


	void OnFingerStationaryBack( FingerMotionEvent e )
	{
		if ( e.Phase == FingerMotionPhase.Updated )
		{

			if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
			{
				if ( e.Finger.DistanceFromStart < inkDict[e.Finger.Index].affectRange() ) {
					inkDict[e.Finger.Index].Spread(Time.deltaTime);
				}
				else {
					inkDict[e.Finger.Index].Fade();
					inkDict[e.Finger.Index] = null;
				}	
			}
		}
	}

	void OnFingerDownBack( FingerDownEvent e )
	{
		GameObject selection = e.Selection;
		if ( e.Finger.Phase == FingerGestures.FingerPhase.Begin )
		{
			CreatInk( e.Position , e.Finger );
		}

	}

	void CreatInk( Vector3 posFinger , FingerGestures.Finger finger )
	{
		GameObject ink = Instantiate ( inkPrefab ) as GameObject;
		ink.transform.parent = LogicManager.LevelManager.GetLevelObject().transform;
		Vector3 pos = Camera.main.ScreenToWorldPoint( posFinger );
		pos .z = 0;
		ink.transform.position = pos;

		Ink inkCom = ink.GetComponent<Ink>();
	
		if ( inkDict.ContainsKey( finger.Index ))
		{
			if (  inkDict[finger.Index] != null )
				inkDict[finger.Index].Fade();
			inkDict[finger.Index] = inkCom;
		}
		else
			inkDict.Add( finger.Index , inkCom );
	}

	void OnFingerUpBack( FingerUpEvent e )
	{
		if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
		{
			inkDict[e.Finger.Index].Fade();
			inkDict[e.Finger.Index] = null;
		}
	}

	void OnFingerHoverBack( FingerHoverEvent e )
	{
		if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
		{
			if ( e.Finger.DistanceFromStart < inkDict[e.Finger.Index].affectRange() ) {
				inkDict[e.Finger.Index].Spread(Time.deltaTime);
			}
			else {
				inkDict[e.Finger.Index].Fade();
				inkDict[e.Finger.Index] = null;
			}	
		}

	}


	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;


		Vector3 accessTopRight = Camera.main.ScreenToWorldPoint( Vector3.zero );
		Vector3 accessBotLeft = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width , Screen.height ));
		Vector3 accesableSize = accessBotLeft - accessTopRight;
		accesableSize.x += frame.x ;
		accesableSize.y += frame.y ;

		Gizmos.DrawWireCube( transform.position + frameOffset , accesableSize );

		Gizmos.color = new Color( 1f ,  0.5f , 0.5f );

		Vector3 initFrame = initPos;
		initFrame.z = Global.CAMERA_INIT_POSITION.z;
		Gizmos.DrawWireCube( initFrame , accessBotLeft - accessTopRight );

	}
}
