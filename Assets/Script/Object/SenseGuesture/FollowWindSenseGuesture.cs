using UnityEngine;
using System.Collections;

public class FollowWindSenseGuesture : SenseGuesture {

	[SerializeField] FollowWind followWind;
	[SerializeField] MaxMin velocityRange;

	void Awake()
	{
		if ( followWind == null )
			followWind = GetComponent<FollowWind>();
	}

	public override void DealSwipe (SwipeGesture guesture)
	{
		if ( followWind != null )
		{
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint( guesture.StartPosition );
			touchPosition.z = transform.position.z;
			float velocity = Mathf.Clamp( guesture.Velocity , velocityRange.min , velocityRange.max );


			followWind.AddImpuse( Global.V2ToV3(guesture.Move) * velocity
				, touchPosition  );
		}
	}
}
