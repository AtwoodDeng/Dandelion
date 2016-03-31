using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PointArea : Area {

	[SerializeField] SpriteRenderer sprite;

	public bool isFinished = false;

	void Awake()
	{
		gameObject.tag = "FinalPoint";
		if ( sprite == null )
			sprite = GetComponent<SpriteRenderer>();
		if ( col == null )
			col = GetComponent<Collider2D>();
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn, OnOthersGrowFlower);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn, OnOthersGrowFlower);
	}

	void OnOthersGrowFlower(Message msg)
	{	
		PetalInfo info = (PetalInfo) msg.GetMessage("info");
		if ( info.affectPointArea == this )
		{
			Disappear();
		}
	}

	void Disappear()
	{
		if ( sprite != null )
		{
			sprite.DOFade( 0 , 1f );
		}
		col.enabled = false;
		isFinished = true;
	}

}
