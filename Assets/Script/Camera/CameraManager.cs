using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraManager : MonoBehaviour {


	public CameraManager() { s_Instance = this; }
	public static CameraManager Instance { get { return s_Instance; } }
	private static CameraManager s_Instance;

//	public float Boundary = 0.1f;
//	public float speed = 5f;


	public Vector3 frame;
	public Vector3 frameOffset;
	public Vector3 initPos = Vector3.zero;
	public float senseIntense = 0.2f;

	[SerializeField] float EndFadeSize = 10f;
	[SerializeField] float EndFadeTime = 2f;
	[SerializeField] float EndFadeDelay = 0;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
	}

	void OnEndLevel(Message msg )
	{
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
		
	void OnFingerMove( FingerMotionEvent e )
	{
		if ( enabled )
		{
			if ( e.Phase == FingerMotionPhase.Updated && e.Finger.Index == 1 )
			{
				{
					Vector3 center = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 2 , Screen.height /2  ) );

					Vector3 pos = transform.position;
					// move the camera
					pos -= Global.V2ToV3( e.Finger.DeltaPosition ) * 0.01f * senseIntense;
					// restrict the range
					pos.x = Mathf.Clamp( pos.x , frameOffset.x - frame.x / 2f ,  frameOffset.x + frame.x / 2f );
					pos.y = Mathf.Clamp( pos.y , frameOffset.y - frame.y / 2f ,  frameOffset.y + frame.y / 2f );

					transform.position = pos;
				}

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
