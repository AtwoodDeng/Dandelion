﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

	static public Vector3 GetRandomDirectionV3()
	{
		float theta = Random.Range(0, Mathf.PI * 2f );
		float beta = Random.Range(0, Mathf.PI * 2f );
		return new Vector3(Mathf.Sin(theta) * Mathf.Cos(beta)
			,Mathf.Sin(theta) * Mathf.Sin(beta)
			,Mathf.Cos(theta)).normalized;
	}

	static public string LAND_TAG = "Land";

	static public float StandardizeAngle(float angle)
	{
		float res = angle;
		while(res > 180f) res -= 360f;
		while(res <= -180f) res += 360f;
		return res;
	}


	static public int ROCK_ORDER 				= 50;  
	static public int SHADOW_ORDER 				= 40;  
	static public int FLOWER_STEM_ORDER 		= 10;
	static public int FLOWER_LEAF_ORDER 		= 8;
	static public int LIGHT_ORDER 				= 0;
	static public int WIND_ARROW_ORDER 			= -25;
	static public int WIND_BACK_ORDER 			= -30;

	static public float WIND_UI_Z = 0f;

	static public string[] levelNames =
	{
		"lvl1",
		"lvl1-3",
		"lvl1-2",

	};

	static public float GaussSigma = 2.5f;
	static public int GaussSize = 9;

	static public float[] GaussValue = {
		0.002333f,	0.004054f,	0.006015f,	0.007623f,	0.008249f,	0.007623f,	0.006015f,	0.004054f,	0.002333f,
		0.004054f,	0.007044f,	0.010453f,	0.013247f,	0.014335f,	0.013247f,	0.010453f,	0.007044f,	0.004054f,
		0.006015f,	0.010453f,	0.015512f,	0.019657f,	0.021272f,	0.019657f,	0.015512f,	0.010453f,	0.006015f,
		0.007623f,	0.013247f,	0.019657f,	0.02491f,	0.026956f,	0.02491f,	0.019657f,	0.013247f,	0.007623f,
		0.008249f,	0.014335f,	0.021272f,	0.026956f,	0.02917f,	0.026956f,	0.021272f,	0.014335f,	0.008249f,
		0.007623f,	0.013247f,	0.019657f,	0.02491f,	0.026956f,	0.02491f,	0.019657f,	0.013247f,	0.007623f,
		0.006015f,	0.010453f,	0.015512f,	0.019657f,	0.021272f,	0.019657f,	0.015512f,	0.010453f,	0.006015f,
		0.004054f,	0.007044f,	0.010453f,	0.013247f,	0.014335f,	0.013247f,	0.010453f,	0.007044f,	0.004054f,
		0.002333f,	0.004054f,	0.006015f,	0.007623f,	0.008249f,	0.007623f,	0.006015f,	0.004054f,	0.002333f,

		};

	static public string NextLevel()
	{
		string nowLevel = SceneManager.GetActiveScene().name;
		int i = 0 ;
		while( i < levelNames.Length && levelNames[i] != nowLevel )
			i ++;
		return levelNames[(i+1) % levelNames.Length];

	}

	static public string BGM_PATH = "Prefab/System/BGM";

	static public Vector3 CAMERA_INIT_POSITION = new Vector3( 0  , 0 , -10f);

	static float m_pixel2Unit = -1f;
	static public float Pixel2Unit
	{
		get {
			if ( m_pixel2Unit > 0 )
				return m_pixel2Unit;
			
			if ( Camera.main != null )
			{
				Vector3 zero = Camera.main.ScreenToWorldPoint( new Vector3( 0 , 0 , 0 ));
				Vector3 one = Camera.main.ScreenToWorldPoint( new Vector3( 1f , 0 , 0 ));
				m_pixel2Unit = ( one.x - zero.x ) / 1f ;
			}
			else
				return 0.01f;
			return m_pixel2Unit;
		}
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
	FlyAway,
	Init,
}
[System.SerializableAttribute]
public struct WindSensablParameter
{
	public bool shouldStore;
	public bool shouldUpdate;
}

public enum PetalType
{
	Normal,
	Final,
}

[System.Serializable]
public struct PetalInfo
{
	public Vector3 position;
	public PetalType type;
	public Area affectPointArea;
	public Transform parent;
}

[System.Serializable]
public enum CameraState
{
	Disable,
	Free,
	FollowTrans,
}