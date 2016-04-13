using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {


	public CameraManager() { s_Instance = this; }
	public static CameraManager Instance { get { return s_Instance; } }
	private static CameraManager s_Instance;

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

	public Vector3 OffsetFromInit{
		get {
			return  transform.position - initPos;
		}
	}

	bool canMove = false;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
	}

	void OnFirstFlower( Message msg )
	{
		canMove = true;
	}

	void OnEndLevel(Message msg )
	{
		canMove = false;
		//fade This
		GetComponent<Camera>().DOOrthoSize( EndFadeSize , EndFadeTime ).SetDelay( EndFadeDelay );
		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			c.DOOrthoSize( EndFadeSize , EndFadeTime ).SetDelay( EndFadeDelay );
		}
		transform.DOMove( Global.CAMERA_INIT_POSITION , EndFadeTime ).SetDelay(EndFadeDelay);
	}



	void Awake()
	{
		initPos.z = Global.CAMERA_INIT_POSITION.z;
		transform.position = initPos;
	}

	void Update () {
//		Vector3 move = Vector3.zero;
//		if (Input.mousePosition.x > Screen.width *(1f -Boundary))
//	     {
//	         move.x += speed * Time.deltaTime;
//	     }
//	     
//	     if (Input.mousePosition.x < Screen.width *(Boundary))
//	     {
//	         move.x -= speed * Time.deltaTime;
//	     }
//	     
//	     // if (Input.mousePosition.y > Screen.height * ( 1f - Boundary))
//	     // {
//	     //     move.y += speed * Time.deltaTime;
//	     // }
//	     
//	     // if (Input.mousePosition.y < Screen.height * Boundary )
//	     // {
//	     //     move.y -= speed * Time.deltaTime;
//	     // }
//	 	offset += move;

//	 	transform.position = transform.position + move;
	}

	void OnDoubleTapBack( TapGesture g )
	{
		WindAdv wind = LogicManager.LevelManager.GetWind();
		wind.UISwitch();

	}
		
	void OnFingerMoveBack( FingerMotionEvent e )
	{
		if ( enabled && canMove )
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
					// Vector3 center = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 2 , Screen.height /2  ) );

					Vector3 pos = transform.position;
					// move the camera
					pos -= Global.V2ToV3( e.Finger.DeltaPosition ) * 0.01f * senseIntense;
					// restrict the range
					pos.x = Mathf.Clamp( pos.x , frameOffset.x - frame.x / 2f ,  frameOffset.x + frame.x / 2f );
					pos.y = Mathf.Clamp( pos.y , frameOffset.y - frame.y / 2f ,  frameOffset.y + frame.y / 2f );

					transform.position = pos;
				}else if ( e.Phase == FingerMotionPhase.Ended )
				{
					Debug.Log("End and Update wind");
					LogicManager.LevelManager.GetWind().StartUpdateWind();
				}
			}

		}
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
			GameObject ink = Instantiate ( inkPrefab ) as GameObject;
			ink.transform.parent = LogicManager.LevelManager.GetLevelObject().transform;
			Vector3 pos = Camera.main.ScreenToWorldPoint( e.Position );
			pos .z = 0;
			ink.transform.position = pos;

			Ink inkCom = ink.GetComponent<Ink>();

			if ( inkDict.ContainsKey( e.Finger.Index ))
			{
				if (  inkDict[e.Finger.Index] != null )
					inkDict[e.Finger.Index].Fade();
				inkDict[e.Finger.Index] = inkCom;
			}
			else
				inkDict.Add( e.Finger.Index , inkCom );
		}

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
