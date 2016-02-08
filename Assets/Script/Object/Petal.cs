using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Petal : MonoBehaviour {

	protected Flower flower;

	[SerializeField] protected GameObject flowerPrefab;
	[SerializeField] float minGrowDistance = 0.3f;

	public PetalState state = PetalState.Link;

	public struct FlowerGrowInfo
	{
		public Vector3 position;
	}

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


	List<FlowerGrowInfo> flowerGrowInfoList = new List<FlowerGrowInfo>();
	void OnOthersGrowFlower(Message msg)
	{	
		FlowerGrowInfo info = (FlowerGrowInfo) msg.GetMessage("info");

		flowerGrowInfoList.Add(info);
	}


	virtual public void Init(Flower _flower, int index)
	{
		// link the parent flower
		flower = _flower;
		//initialize the init rotation
		transform.localRotation = Quaternion.EulerAngles(new Vector3(0,0, 2f * Mathf.PI * index / flower.petalNum));

	}

	// grow a new flower on the position
	protected void GrowFlowerOn(Vector3 position,Transform parent = null)
	{

		GameObject flowerObj = Instantiate(flowerPrefab) as GameObject;
		Flower flowerCom = flowerObj.GetComponent<Flower>();
		flowerObj.transform.position = position - flowerCom.root.localPosition;
		if (parent != null )
			flowerObj.transform.parent = parent;

		SendGrowMessage(position);
	}

	virtual protected void SendGrowMessage(Vector3 position)
	{
		Message growMsg = new Message();
		FlowerGrowInfo info = new FlowerGrowInfo();
		info.position = position;
		growMsg.AddMessage("info", info);
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
		if(coll.gameObject.tag == Global.LAND_TAG)
		{
			OnLand(coll);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (state != PetalState.Fly)
			return;
		if(coll.gameObject.tag == Global.LAND_TAG)
		{
			OnLand(coll);
		}
	}

	virtual public void OnLand(Collision2D coll)
	{
		//Change the State of the petal
		state = PetalState.Land;
		//Grow a new flower on the collision point
		Vector3 contactPoint = new Vector3( coll.contacts[0].point.x , coll.contacts[0].point.y , 0 );

		if (checkCanGrowFlower(contactPoint))
			GrowFlowerOn(contactPoint,coll.collider.transform);
		//selfDestory
		SelfDestory();
	}

	virtual protected bool checkCanGrowFlower(Vector3 position)
	{
		foreach( FlowerGrowInfo info in  flowerGrowInfoList)
		{
			if ( (info.position-position).magnitude < minGrowDistance )
			{
				return false;
			}
		}
		return true;
	}

	virtual public void OnLand(Collider2D coll)
	{
		//Change the State of the petal
		state = PetalState.Land;
		//Grow a new flower on the collision point
		Vector3 contactPoint = transform.position;
		if (checkCanGrowFlower(contactPoint))
			GrowFlowerOn(contactPoint,coll.transform);
		//selfDestory
		SelfDestory();
	}

	virtual protected void SelfDestory()
	{
		 Destroy(this.gameObject);

	}

	// Called by Flower.cs 
	// Call when player blow the dendalion
	virtual public void Blow(Vector2 move, float vel, BlowType blowType = BlowType.Normal)
	{
		if (blowType.Equals(BlowType.Normal))
			state = PetalState.Fly;
		else if (blowType.Equals(BlowType.FlyAway))
			state = PetalState.FlyAway;
	}


}
