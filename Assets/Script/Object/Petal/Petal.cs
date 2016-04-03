using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Petal : MonoBehaviour  {

	protected Flower flower;
	
	[SerializeField] protected GameObject flowerPrefab;
	[SerializeField] float minGrowDistance = 0.3f;
	[SerializeField] int growLimit;


	public PetalState state = PetalState.Link;




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
		transform.localRotation = Quaternion.EulerAngles(new Vector3(0,0, 2f * Mathf.PI * rotation));

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

		flowerObj.transform.position = position - flowerCom.root.localPosition;
		if (parent != null )
			flowerObj.transform.parent = parent;

		SendGrowMessage(position);
	}

	virtual protected void SendGrowMessage(Vector3 position)
	{
		Message growMsg = new Message();
		myGrowInfo.position = position;
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
		if (state != PetalState.Fly)
			return;
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

	virtual public void OnLand(Collision2D coll)
	{

		Debug.Log("On Land");
		if ( state == PetalState.Fly )
		{
			
			//Change the State of the petal
			state = PetalState.Land;
			//Grow a new flower on the collision point
			Vector3 contactPoint = new Vector3( coll.contacts[0].point.x , coll.contacts[0].point.y , 0 );
			Vector3 normal = new Vector3( coll.contacts[0].normal.x , coll.contacts[0].normal.y , 0 );
			Vector3 growPoint = contactPoint - normal * 1f;


			//cast a ray to find if actual hit point
			int layerMask = 1 << LayerMask.NameToLayer("Land");
			RaycastHit2D hitInfo = Physics2D.Raycast(coll.contacts[0].point, Vector2.down , 2f , layerMask );
			if ( hitInfo != null)
			{
				growPoint = Global.V2ToV3( hitInfo.point ) + Vector3.down * hitInfo.distance * 2;
			}

			if (checkCanGrowFlower(growPoint , normal))
				GrowFlowerOn(growPoint, normal , coll.collider.transform );

			transform.DOScale( 0 , 1f ).OnComplete(SelfDestory);

		}else if ( state == PetalState.FlyAway)
		{
			transform.DOScale( 0 , 1f ).OnComplete(SelfDestory);
		}
	}

	virtual protected bool checkCanGrowFlower(Vector3 position , Vector3 normal )
	{
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
		// Destroy(this.gameObject);

	}

	// Called by Flower.cs 
	// Call when player blow the dendalion
	virtual public void Blow(Vector2 move, float vel, BlowType blowType = BlowType.Normal)
	{
		if (blowType == BlowType.Normal )
			state = PetalState.Fly;
		else if (blowType == BlowType.FlyAway)
			state = PetalState.FlyAway;
	}

	


}
