using UnityEngine;
using System.Collections;

public class FlowerTopSenseGuesture : SenseGuesture {
	[SerializeField] Flower flower;


	public override void DealSwipe (SwipeGesture guesture)
	{
		float petalNumBefore = flower.GetPetalNumByType(PetalState.Link);
		flower.Blow( guesture.Move.normalized , guesture.Velocity * Global.Pixel2Unit );
		float petalNumAfter = flower.GetPetalNumByType(PetalState.Link);

		BoxCollider col = GetComponent<BoxCollider>();
		if ( col != null )
		{
			float petalChange = 1.0f * ( petalNumBefore + 20 ) / ( petalNumAfter + 20 );
			Vector3 size = col.size;
			size /= petalChange;
			if ( petalNumAfter == 0 )
				size = Vector3.zero ;
			size.z = Mathf.Clamp( size.z , 1f , 9999f);
			col.size = size;
		}
	}

}
