using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flower : MonoBehaviour {

	[SerializeField] List<GameObject> petalPrefabList = new List<GameObject>();
	[SerializeField] MaxMinInt petalNumRange;
	[SerializeField] Transform petalRoot;

	[SerializeField] List<Petal> petals;


	public int petalNum=-1;
	public Rigidbody2D stem;
	public Transform root;

	void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (petalNum <= 0 )
			petalNum = Random.Range(petalNumRange.min,petalNumRange.max);

		for(int i = 0 ; i < petalNum ; ++ i )
		{
			GameObject petalObj = Instantiate(petalPrefabList[Random.Range(0, petalPrefabList.Count)]) as GameObject;
			petalObj.transform.parent = petalRoot;
			petalObj.transform.localPosition = Vector3.zero;

			Petal petal = petalObj.GetComponent<Petal>();
			petal.Init(this,i);
			petals.Add(petal);
		}

	}

	public void Blow(Vector2 move , float velocity)
	{
		// Debug.Log("move" + move.ToString() + " vel " + velocity.ToString());

		foreach(Petal petal in petals)
		{
			petal.Blow(move, velocity);
		}
	}

}
