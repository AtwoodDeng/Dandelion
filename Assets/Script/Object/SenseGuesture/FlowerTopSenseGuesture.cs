using UnityEngine;
using System.Collections;

public class FlowerTopSenseGuesture : SenseGuesture {
	[SerializeField] Flower flower;

	public override void DealSwipe (SwipeGesture guesture)
	{
		flower.Blow( guesture.Move , guesture.Velocity );
	}

}
