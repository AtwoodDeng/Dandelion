using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Petal3D : Petal {



    [SerializeField] float mass=12f;
    [SerializeField] float rotationMass=0.001f;
    [SerializeField] float blowIntense=0.003f;
    [SerializeField] float drag = 0.08f;
    [SerializeField] float rotationDrag = 0.05f;
    // [SerializeField] float raiseK = 1f;
    [SerializeField] float gravity = 0.003f;
    [SerializeField] float maxVel = 0.7f;
    [SerializeField] float maxRotVel = 0.66f;

    [SerializeField] Transform petalModel;
    [SerializeField] float petalModelRotateIntense = 0.01f;
    [SerializeField] float petalModelLinkIntense = 0.01f;
    [SerializeField] float petalModelLinkInterval = 2f;

    [SerializeField] MaxMin flyAwayFadeTime;
    float petalModelLinkInit;
    Vector3 petalModelRotateToward;

    Vector3 myUpDiff;
	public override void Init (Flower _flower, int index) {

		Debug.Log("init petal");
        base.Init(_flower, index);
        myUpDiff = Global.GetRandomDirectionV3() * 0.1f;
        chaosFunction = StartCoroutine(GenerateChaos(0.1f));

        petalModelRotateToward = Global.GetRandomDirectionV3();

        // the initial phase position of the petal
        petalModelLinkInit = Random.Range(0,Mathf.PI*2f);
        // randomize some parameter
        petalModelRotateIntense *= Random.Range(0.5f, 2f);
        petalModelLinkIntense *= Random.Range(0.1f, 5f);
        petalModelLinkInterval *= Random.Range(0.5f, 2f);

        rigidbody = GetComponent<Rigidbody2D>();

        //Change the alpha of the petal
        Renderer render = petalModel.GetComponent<Renderer>();
        render.material.DOFade( Random.Range(0.15f,0.5f) , 0.01f);

        //play the animation
        Grow();
        // petalModel.localRotation.SetEulerAngles(new Vector3(Random.Range(0, 360),Random.Range(0, 360),Random.Range(0, 360)));
    }

    public void Grow()
    {
    	float growTime = Random.Range(1f, 2.5f);
    	petalModel.DORotate(Random.rotation.eulerAngles, growTime);
    	Vector3 tempScale = petalModel.transform.localScale ;
    	petalModel.transform.localScale *= 0.001f;

    	petalModel.transform.DOScale(tempScale, growTime);
    }

    public override void AddForce (Vector2 force) {
        myForce += force;
    }
    
    public override void Blow (Vector2 move, float vel , BlowType blowType = BlowType.Normal) {

    	base.Blow(move, vel);
    	AddForce((move.normalized + 0.4f * Global.GetRandomDirection()).normalized * vel * blowIntense );

        if ( blowType.Equals(BlowType.Normal) )
        {

        	Collider2D[] colliders = GetComponents<Collider2D>();
        	foreach(Collider2D c in colliders)
        	{
        		c.isTrigger = false;
        	}

        	colliders = GetComponentsInChildren<Collider2D>();
        	foreach(Collider2D c in colliders)
        	{
        		c.isTrigger = false;
        	}
        }
        else if (blowType.Equals(BlowType.FlyAway))
        {
            Renderer render = petalModel.GetComponent<Renderer>();

            float fadeTime = Random.Range( flyAwayFadeTime.min , flyAwayFadeTime.max);
            petalModel.transform.DOScale(0, fadeTime);
            render.material.DOFade( 0 , fadeTime);
        }
    }

    //Chaos


    void LateUpdate()
    {
        UpdateForce();
    }


    Vector2 myVelocity = Vector2.zero;
    float myRotationVelocity = 0;
    Vector2 myForce = Vector2.zero;

    Rigidbody2D rigidbody;

    protected void UpdateForce()
    {

        // add chaos to force
        myForce += chaosForce;

        // update the velocity
        myVelocity += myForce / mass - new Vector2(0,gravity);

        Vector3 myUp = transform.up + myUpDiff;
        
        //Rotate the petal
        myRotationVelocity = (  Vector3.Cross(myUp, myForce).z 
                                + Vector3.Cross(- myUp , Vector3.down * rotationMass * 0.2f ).z ) / rotationMass;

        //do nothing if the petal is still link to the flower
        if (state == PetalState.Link)
        {
            myVelocity = Vector3.zero;
            myRotationVelocity = 0;
            petalModel.Rotate( petalModelRotateToward * petalModelLinkIntense * Mathf.Sin( petalModelLinkInterval * Time.time + petalModelLinkInit) );
        }

        else if (state == PetalState.Fly )
        {
	         // control the velocity
	        
	        if (myVelocity.magnitude > maxVel)
	            myVelocity = myVelocity.normalized * maxVel;

	        // update the position and rotation
	        myRotationVelocity = Mathf.Clamp(myRotationVelocity, maxRotVel, -maxRotVel);
	        transform.position += Global.V2ToV3(myVelocity);
	        transform.Rotate(Vector3.back, myRotationVelocity);

	        petalModel.Rotate( petalModelRotateToward * myRotationVelocity * petalModelRotateIntense);

	        // drag the velocity
	        myVelocity *= ( 1f - drag );
	        myRotationVelocity *= (1f - rotationDrag);

	        // set force back to zero
	        myForce = Vector2.zero;
    	}

    	// rigidbody velocity don't effect;
    	if (rigidbody != null)	
 		   	rigidbody.velocity = Vector2.zero;

    }


    Vector2 chaosForce;
    Coroutine chaosFunction;
    // update the chaos(this will run forever)
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
        chaosFunction = StartCoroutine(GenerateChaos(Random.Range(.5f, .8f)));
    }

    public override void OnLand(Collider2D coll)
    {
    	base.OnLand(coll);

    }


    protected override void SelfDestory () {
        // base.SelfDestory();
        StopCoroutine(chaosFunction);

        Renderer render = petalModel.GetComponent<Renderer>();
        if (render != null)
        {
        	render.material.DOFade(0, 1f).OnComplete(base.SelfDestory);
        }
    }
}
