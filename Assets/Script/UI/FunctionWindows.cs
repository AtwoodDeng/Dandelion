﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FunctionWindows : MonoBehaviour {

	[SerializeField] Text NumberText;

	void Update()
	{
		NumberText.text = LogicManager.Instance.RemainBlowTime.ToString();
	}
}
