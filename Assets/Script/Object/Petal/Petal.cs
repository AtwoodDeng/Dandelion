using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Petal : MonoBehaviour  {

	protected Flower flower;
	
	[SerializeField] protected GameObject flowerPrefab;
	[SerializeField] float minGrowDistance = 0.3f;
	[SerializeField] int growLimit;
	[SerializeField] public Transform top;

	public PetalState state = PetalState.Link;

	public int ID
	{
		get {
			return m_ID;
		}
	}
	int m_ID;
	static int m_ID_Pool=0;

	public PetalInfo myGrowInfo;

	public enum BlowType
	{
		Normal,
		FlyAway
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn, OnOthersGrowFlower);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn, OnOthersGrowFlower);
	}


	List<PetalInfo> flowerGrowInfoList = new List<PetalInfo>();
	void OnOthersGrowFlower(Message msg)
	{	
		PetalInfo info = (PetalInfo) msg.GetMessage("info");
		flowerGrowInfoList.Add(info);
	}

	void Awake()
	{
		m_ID = m_ID_Pool++;
	}

	virtual public void Init(Flower _flower, int index)
	{
		// link the parent flower
		flower = _flower;
		//initialize the init rotation
		Grow(index);
	}

	public virtual void Grow( int index )
	{
		float rotation = 0.5f;
		if ( flower != null )
			rotation = index / flower.petalNum;
		transform.localRotation = Quaternion.Euler(new Vector3(0,0, 2f * Mathf.PI * rotation));

	}

	public virtual float GetGrowTime()
	{
		return 0;
	}

	// grow a new flower on the position
	protected void GrowFlowerOn(Vector3 position, Vector3 normal , Transform parent = null  )
	{
		GameObject flowerObj = Instantiate(flowerPrefab) as GameObject;
		Flower flowerCom = flowerObj.GetComponent<Flower>();
		flowerCom.growParameter.normal = normal;
		flowerCom.growParameter.petalType = myGrowInfo.type;
		flowerCom.Init();

		if (parent != null )
			flowerObj.transform.SetParent(parent);
		
		flowerObj.transform.position = position - flowerCom.root.localPosition;
		// set z to zero 


		SendGrowMessage(position,parent);
	}

	virtual protected void SendGrowMessage(Vector3 position,Transform parent)
	{
		Message growMsg = new Message();
		myGrowInfo.position = position;
		myGrowInfo.parent = parent;
		growMsg.AddMessage("info", myGrowInfo);
		EventManager.Instance.PostEvent(EventDefine.GrowFlowerOn, growMsg,this);
	}


	// Called by Wind.cs
	// Called every frame when wind force the petal
	virtual public void AddForce(Vector2 force)
	{
	}


    void OnCollisionEnter2D(Collision2D coll)
	{
		Land land = coll.gameObject.GetComponent<Land>();
		if(land != null || coll.gameObject.tag == Global.LAND_TAG)
		{
			OnLand(coll);
		}
	}


	void OnCollisionEnter(Collision coll)
	{
		Land land = coll.gameObject.GetComponent<Land>();
		if(land != null || coll.gameObject.tag == Global.LAND_TAG)
		{
			OnLand(coll);
		}
	}


//	void OnTriggerEnter2D(Collider2D coll)
//	{
//		if (state != PetalState.Fly)
//			return;
//		Land land = coll.gameObject.GetComponent<Land>();
//		if(land != null || coll.gameObject.tag == Global.LAND_TAG)
//		{
//			OnLand(coll);
//		}
//	}

	public void OnLand(Collision2D coll)
	{
		OnLand( Global.V2ToV3( coll.contacts[0].point )
			, Global.V2ToV3( coll.contacts[0].normal ) 
			, coll.collider.gameObject );
	}

	public void OnLand(Collision coll)
	{
		OnLand( coll.contacts[0].point , coll.contacts[0].normal , coll.collider.gameObject );
	}

	Message destoryMessage = new Message();

	virtual public void OnLand(Vector3 point , Vector3 normal , GameObject obj)
	{
		Debug.Log("OnLand " + name + " " + obj.name );
		if ( state == PetalState.Fly || state == PetalState.Init )
		{
			// set the petal stable
			if ( GetComponent<Rigidbody>() != null ) {
				GetComponent<Rigidbody>().isKinematic = true;
			}

			//Change the State of the petal
			if ( state == PetalState.Init )
				EventManager.Instance.PostEvent( EventDefine.GrowFirstFlower );
			state = PetalState.Land;

			//check the land
			Land land = obj.GetComponent<Land>();

			//Grow a new flower on the collision point
			Vector3 contactPoint = new Vector3( point.x , point.y , 0 );
			Vector3 _normal = new Vector3( normal.x , normal.y , 0 );
			Vector3 growPoint = contactPoint - _normal.normalized * 0.1f;


			//cast a ray to find if actual hit point
			int layerMask = 1 << LayerMask.NameToLayer("Land");
			RaycastHit hitInfo ;

			if ( Physics.Raycast( point , Vector3.down , out hitInfo, 2f , layerMask ) )
			{
				growPoint = Global.V2ToV3( hitInfo.point ) + Vector3.down * hitInfo.distance * 2;
			}

			if (checkCanGrowFlower(growPoint , _normal , land)) 
			{
				GrowFlowerOn(growPoint, _normal , obj.transform );

			} else
			{
				destoryMessage.AddMessage("onLand" , 1);
			}

			transform.DOScale( 0 , 1f ).OnComplete(SelfDestory);
			transform.DOMove( - 0.1f * _normal , 1f ).SetRelative(true).OnComplete(SelfDestory);

		}else if ( state == PetalState.FlyAway)
		{
			transform.DOScale( 0 , 1f ).OnComplete(SelfDestory);
		}
	}

	virtual protected bool checkCanGrowFlower(Vector3 position , Vector3 normal , Land land = null )
	{
		if ( land != null && !land.IfCanGrowFlower() )
			return false;
		
		if ( Vector3.Dot( Vector3.up , normal.normalized ) < 0.3f )
			return false;
		
		if (flowerGrowInfoList.Count >= growLimit)
			return false;
			
		foreach( PetalInfo info in  flowerGrowInfoList)
		{
			if ( (info.position-position).magnitude < minGrowDistance )
			{
				return false;
			}
		}
		return true;
	}

//	virtual public void OnLand(Collider2D coll)
//	{
//		if ( state == PetalState.Fly )
//		{
//			
//			//Change the State of the petal
//			state = PetalState.Land;
//			//Grow a new flower on the collision point
//			Vector3 contactPoint = transform.position;
//
//
//			if (checkCanGrowFlower(contactPoint))
//				GrowFlowerOn(contactPoint,coll.transform);
//			//selfDestory
//			SelfDestory();
//		}
//	}

	virtual protected void SelfDestory()
	{
		transform.position = new Vector3(999999f,999999f,999999f);
		gameObject.SetActive(false);


		destoryMessage.AddMessage("petal" , this );
		EventManager.Instance.PostEvent(EventDefine.PetalDestory , destoryMessage );
		// Destroy(this.gameObject);

	}

	// Called by Flower.cs 
	// Call when player blow the dendalion
	public void Blow(Vector2 move, float vel, BlowType blowType = BlowType.Normal)
	{
		Blow( move.normalized * vel , blowType );
	}
	virtual public void Blow(Vector2 vel, BlowType blowType = BlowType.Normal)
	{
		if (blowType == BlowType.Normal )
			state = PetalState.Fly;
		else if (blowType == BlowType.FlyAway)
			state = PetalState.FlyAway;
	}


}
