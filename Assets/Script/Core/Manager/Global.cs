using UnityEngine;
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

	static public Vector3 GetRandomDirectionV3()
	{
		float theta = Random.Range(0, Mathf.PI * 2f );
		float beta = Random.Range(0, Mathf.PI * 2f );
		return new Vector3(Mathf.Sin(theta) * Mathf.Cos(beta)
			,Mathf.Sin(theta) * Mathf.Sin(beta)
			,Mathf.Cos(theta));
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

	static public float WIND_UI_Z = 2f;

	static string[] levelNames =
	{
		"lvl1",
		"lvl1-2",
		"lvl1-3",
		"lvl1-4",
		"lvl1-5",
		"lvl2",

	};

	static public string NextLevel()
	{
		string nowLevel = Application.loadedLevelName;
		int i = 0 ;
		while( i < levelNames.Length && levelNames[i] != nowLevel )
			i ++;
		return levelNames[(i+1) % levelNames.Length];

	}

	static public string BGM_PATH = "Prefab/System/BGM";

	static public Vector3 CAMERA_INIT_POSITION = new Vector3( 0  , 0 , -10f);
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
}