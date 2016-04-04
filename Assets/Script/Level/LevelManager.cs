using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	[SerializeField] protected GameObject level;
	[SerializeField] protected int blowTime = 999;
	[SerializeField] protected List<PointArea> pointAreas = new List<PointArea>();
	[SerializeField] protected WindAdv wind;


	virtual public GameObject GetLevelObject()
	{
		if ( level != null )
			return level;
		return GameObject.Find("level");
	}

	public bool CheckLevelFinished()
	{
		if ( pointAreas == null || pointAreas.Count <= 0 )
		{
			GameObject[] areas = GameObject.FindGameObjectsWithTag("FinalPoint");
			foreach( GameObject area in areas )
			{
				if ( area.GetComponent<PointArea>() != null )
				{
					pointAreas.Add( area.GetComponent<PointArea>());
				}
			}
		}

		foreach( PointArea pa in pointAreas )
		{
			if ( ! pa.isFinished )
				return false;
		}
		return true;
	}

	public WindAdv GetWind()
	{
		if ( wind != null ) 
			return wind;
		GameObject windObj = GameObject.FindWithTag( "Wind");

		if ( windObj != null )
		{
			wind = windObj.GetComponent<WindAdv>();
		}
		return wind;
	}

	virtual public int GetBlowTime()
	{
		return blowTime;
	}

	virtual public int GetEvaluation()
	{
		return 3;
	}
}
