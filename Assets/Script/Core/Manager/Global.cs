﻿using UnityEngine;
using System.Collections;

public class Global {

	static public Vector2 V3ToV2(Vector3 v)
	{
		return new Vector2(v.x,v.y);
	}
	static public Vector3 V2ToV3(Vector2 v)
	{
		return new Vector3(v.x,v.y,0);
	}
	static public Vector2 GetRandomDirection()
	{
		float angle = Random.Range(0, Mathf.PI * 2 );
		return new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
	}
}

[System.SerializableAttribute]
public struct MaxMin<T>{
	public T max;
	public T min;
}

[System.SerializableAttribute]
public struct MaxMinInt{
	public int max;
	public int min;
}

[System.SerializableAttribute]
public struct MaxMin{
	public float max;
	public float min;
}

public enum PetalState
{
	Link,
	Fly,
	Land,
}