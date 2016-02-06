using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Petal : MonoBehaviour {

	Flower flower;

	[SerializeField] GameObject flowerPrefab;

	public PetalState state = PetalState.Link;

	virtual public void Init(Flower _flower, int index)
	{
		flower = _flower;
		transform.localRotation = Quaternion.EulerAngles(new Vector3(0,0, 2f * Mathf.PI * index / flower.petalNum));

	}

	protected void GrowFlowerOn(Vector3 position,Transform parent = null)
	{
		GameObject flowerObj = Instantiate(flowerPrefab) as GameObject;
		Flower flowerCom = flowerObj.GetComponent<Flower>();
		flowerObj.transform.position = position - flowerCom.root.localPosition;
		if (parent != null )
			flowerObj.transform.parent = parent;

	}

	// Called by Wind.cs
	// Called every frame when wind force the petal
	virtual public void AddForce(Vector2 force)
	{
	}


    void OnCollisionEnter2D(Collision2D coll)
	{
		if (state != PetalState.Fly)
			return;
		if(coll.gameObject.tag == "Green")
		{
			OnLand(coll);
		}
	}

	virtual public void OnLand(Collision2D coll)
	{
		state = PetalState.Land;
		Vector3 contactPoint = new Vector3( coll.contacts[0].point.x , coll.contacts[0].point.y , 0 );
		GrowFlowerOn(contactPoint,coll.collider.transform);
		Destroy(this.gameObject);

	}

	// Called by Flower.cs 
	// Call when player blow the dendalion
	virtual public void Blow(Vector2 move, float vel)
	{
		state = PetalState.Fly;
	}
}
