using UnityEngine;
using System.Collections;

public class FollowWind : MonoBehaviour , WindSensable {

    // Wind3D tempWind = null;
    [SerializeField] protected float swind; // sense of wind
    [SerializeField] protected float K;     // 
    [SerializeField] protected float drag=1f;
    [SerializeField] protected float initTime = 0.1f;
	[SerializeField] protected float senseImpuse;
	[SerializeField] MaxMin swingRange;
	[SerializeField] protected float controlDrag;

    protected Vector2 windVelocity = Vector2.zero;

    public void SenseWind(Vector2 velocity)
    {
        windVelocity = velocity;
    }

    public void ExitWind(){
        windVelocity = Vector2.zero;
    }

    public Vector3 getPosition() {
        return transform.position;
    }
    
    [SerializeField] public WindSensablParameter windSensablParameter;

    public WindSensablParameter getParameter()
    {
        return windSensablParameter;
    }

	virtual public void AddImpuse(Vector3 impuse , Vector3 position )
	{
		Vector3 m_impuse = impuse;
		m_impuse.z = 0 ;
		Vector3 radius = position - transform.position;
		radius.z = 0;
		float torque = 1e-7f * Vector3.Dot( Vector3.back , Vector3.Cross( radius , m_impuse ));
		torque *= senseImpuse;

		angleVol += torque;
	}

    void Awake()
    {
        // windSensablParameter.onRender = true;
    	// StartCoroutine(UpdateAnimation());
    }
    float timer = 0;

    bool isInit = false;
    float initAngle = 0;
    float angleVol = 0;

    void LateUpdate()
    {
    	timer += Time.deltaTime;
	    if (timer < initTime )
	    	return;

	    if (!isInit)
	    {
            Init();
        }
        // while(true)
        // {
        UpdateObject();
    }

    virtual protected void Init()
    {
            initAngle = Global.StandardizeAngle( transform.eulerAngles.z );
            angleVol = 0;
            isInit = true;
    }

    virtual protected void UpdateObject()
    {
        float windForce = 0;
        //test if in the wind
        if ( windVelocity != Vector2.zero)
        {
       	 	Vector3 windDirect = Global.V2ToV3(windVelocity.normalized);
       		float windEffect = Vector3.Dot(Vector3.back , Vector3.Cross(transform.up, windDirect) );
       		windForce = windEffect * swind * windVelocity.magnitude;
        }

        // do the animation of stem
        
        float offSetAngle = Mathf.DeltaAngle(transform.eulerAngles.z, initAngle);

        float KForce = - offSetAngle * K;

		angleVol += windForce + KForce;
		angleVol *= drag;

		if ( (swingRange.max > 0 && offSetAngle > swingRange.max && angleVol > 0)
			||  (swingRange.min < 0 && offSetAngle < swingRange.min && angleVol < 0 ) )
		{
			angleVol *= controlDrag;
		}

//		if ( (swingRange.max > 0 && offSetAngle > swingRange.max * 2 )
//			||  (swingRange.min < 0 && offSetAngle < swingRange.min * 2 ) )
//		{
//			angleVol *= drag;
//			Debug.Log("Out of range 2" );
//		}


        //Rotate the object
		transform.Rotate(Vector3.back, angleVol * Time.deltaTime * 30f );


        // yield return null;
    // }    
    }

    void OnBecameVisible()
    {
        windSensablParameter.shouldUpdate = true;
        enabled = true;
    }

    void OnBecameInvisible ()
    {
        // windSensablParameter.onRender = false;
        enabled = false;
    }

}
