using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventExtend : MonoBehaviour {

	[SerializeField] EventDefine senseEvent;

	[SerializeField] bool initZero = true;
	[SerializeField] float extendToY;
	[SerializeField] float extendTime = 2f;

	float m_toY;
	void Awake()
	{
		if ( initZero )
		{
			m_toY = transform.localScale.y;
			Vector3 scale = transform.localScale;
			scale.y = 0;
			transform.localScale = scale;
		}else
		{
			m_toY = extendToY;
		}
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(senseEvent, OnEvent);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(senseEvent, OnEvent);
	}

	void OnEvent(Message msg )
	{
		transform.DOScaleY( m_toY , extendTime );
	}
}
