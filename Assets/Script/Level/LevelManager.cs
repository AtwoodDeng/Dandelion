using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	[SerializeField] GameObject level;
	[SerializeField] int blowTime;
	[SerializeField] List<PointArea> pointAreas = new List<PointArea>();

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

	virtual public int GetBlowTime()
	{
		return blowTime;
	}
}
