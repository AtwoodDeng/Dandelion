using UnityEngine;
using System.Collections;

public class PetalAdv : Petal {

	[SerializeField] float mass=1f;
	[SerializeField] float rotationMass=1f;
	[SerializeField] float blowIntense=1f;
	[SerializeField] float drag = 0.01f;
	[SerializeField] float rotationDrag = 0.01f;
	// [SerializeField] float raiseK = 1f;
	[SerializeField] float gravity = 0.1f;
	[SerializeField] float maxVel = 0.1f;
	[SerializeField] float maxRotVel = 0.1f;

	Vector3 myUpDiff;
	public override void Init (Flower _flower, int index) {
		base.Init(_flower, index);
		myUpDiff = Global.V2ToV3 (Global.GetRandomDirection() * 0.33f );
		StartCoroutine(GenerateChaos(0.1f));
    }

	public override void AddForce (Vector2 force) {
		myForce += force;
    }
	
	public override void Blow (Vector2 move, float vel) {
		base.Blow(move, vel);
		AddForce((move.normalized + 0.4f * Global.GetRandomDirection()).normalized * vel * blowIntense );
    }

    Vector2 myVelocity = Vector2.zero;
    float myRotationVelocity = 0;
    Vector2 myForce = Vector2.zero;

    void LateUpdate()
    {
    	// Vector2 raise = Mathf.Pow( myVelocity.x , 3f ) * raiseK * Vector2.up;
    	// Debug.Log("Force " + (myForce * 100f ).ToString() + " raise " + ( raise * 100f ) .ToString());
    	myForce += chaosForce;
    	myVelocity += myForce / mass - new Vector2(0,gravity);

    	Vector3 myUp = transform.up + myUpDiff;
    	myRotationVelocity = (  Vector3.Cross(myUp, myForce).z 
    							+ Vector3.Cross(- myUp , Vector3.down * rotationMass * 0.2f ).z ) / rotationMass;



    	if (state == PetalState.Link)
    	{
    		myVelocity = Vector3.zero;
    		myRotationVelocity = 0;
    	}

    	// Debug.Log("vel " + myVelocity.magnitude * 1000f + " rovel " + myRotationVelocity*1000f);
    	if (myVelocity.magnitude > maxVel)
    		myVelocity = myVelocity.normalized * maxVel;
    	myRotationVelocity = Mathf.Clamp(myRotationVelocity, maxRotVel, -maxRotVel);
    	transform.position += Global.V2ToV3(myVelocity);
    	transform.Rotate(Vector3.back, myRotationVelocity);

    	myVelocity *= ( 1f - drag );
    	myRotationVelocity *= (1f - rotationDrag);

    	myForce = Vector2.zero;
    }

    Vector2 chaosForce;
    IEnumerator GenerateChaos(float time)
    {
    	float timer = 0;
    	chaosForce = Global.GetRandomDirection() * myForce.magnitude * Random.Range(0.1f, 1f);
    	while(true)
    	{
    		chaosForce = (chaosForce.normalized + Global.GetRandomDirection().normalized * 0.1f) * chaosForce.magnitude;

    		timer += Time.deltaTime;
    		if (timer > time)
    			break;
    		yield return null;
    	}
    	StartCoroutine(GenerateChaos(Random.Range(.5f, .8f)));
    }
}
