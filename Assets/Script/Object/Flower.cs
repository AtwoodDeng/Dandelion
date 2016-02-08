using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Flower : MonoBehaviour {

	[SerializeField] List<GameObject> petalPrefabList = new List<GameObject>();
	[SerializeField] MaxMinInt petalNumRange;
	[SerializeField] Transform petalRoot;

	[SerializeField] List<Petal> petals;

	[SerializeField] protected float blowChance = 1f;
	[SerializeField] protected float flyAwayChance = 0.77f;
	[SerializeField] protected Transform stemModel;


	public int petalNum=-1;
	public Transform root;

	[SerializeField] GrowParameter growParameter;

	[System.SerializableAttribute]
	public struct GrowParameter
	{
		public float growTime;
		public MaxMin growPeterInterval;
	}

	void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (petalNum <= 0 )
			petalNum = Random.Range(petalNumRange.min,petalNumRange.max);

		Grow();

	}

	public void CreatePatel()
	{
		GameObject petalObj = Instantiate(petalPrefabList[Random.Range(0, petalPrefabList.Count)]) as GameObject;
		petalObj.transform.parent = petalRoot;
		petalObj.transform.localPosition = Vector3.zero;
		petalObj.transform.localScale = Vector3.one * Random.Range(.9f, 1.1f);

		Petal petal = petalObj.GetComponent<Petal>();
		petal.Init(this,petals.Count);
		petals.Add(petal);

	}


	public void Grow()
	{
		transform.localScale = Vector3.one * 0.001f;
		float ScaleChange = Random.Range(0.33f, 0.8f);
		stemModel.transform.localScale *= ScaleChange;
		petalRoot.transform.localPosition *= ScaleChange;
		transform.DOScale(Vector3.one , growParameter.growTime).OnComplete(GrowPatel);
	}

	public void GrowPatel()
	{
		Debug.Log("Grow petal");
		StartCoroutine(GrowPetalCor());
	}

	IEnumerator GrowPetalCor()
	{
		for(int i = 0 ; i < petalNum ; ++ i)
		{
			CreatePatel();
			yield return new WaitForSeconds(Random.Range(growParameter.growPeterInterval.min, growParameter.growPeterInterval.max));
		}
	}

	public void Blow(Vector2 move , float velocity)
	{
		// Debug.Log("move" + move.ToString() + " vel " + velocity.ToString());

		// make the move direct and velocity a little bit different
		move = (move.normalized + Global.GetRandomDirection() * 0.2f).normalized;
		velocity *= Random.Range(0.7f, 1.3f);
		foreach(Petal petal in petals)
		{
			if (petal.state == PetalState.Link && Random.Range(0, 1f) < blowChance)
			{
				if (Random.Range(0, 1f) < flyAwayChance)
					petal.Blow(move, velocity, Petal.BlowType.FlyAway);
				else
					petal.Blow(move, velocity);
			}
		}
	}

}
