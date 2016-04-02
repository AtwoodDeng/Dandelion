using UnityEngine;
using System.Collections;


public class Area : MonoBehaviour {
	[SerializeField] protected Collider2D col;

	[SerializeField] protected PetalType toType;

	void Awake()
	{
		if ( col == null )
			col = GetComponent<Collider2D>();
	}

	void OnTriggerEnter2D(Collider2D col )
	{
		Petal petal = col.GetComponent<Petal>();
		if ( petal != null )
		{
			petal.myGrowInfo.type = toType;
			petal.myGrowInfo.affectPointArea = this;
		}

	}



}
