﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEvent : MonoBehaviour {

	[SerializeField] EventDefine senseEvent;

	[SerializeField] protected float time = 2f;
	[SerializeField] protected Ease easeType;
	[SerializeField] protected float delay = 0f;


	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(senseEvent, Do);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(senseEvent, Do);
	}

	protected virtual void Do(Message msg )
	{}
}
