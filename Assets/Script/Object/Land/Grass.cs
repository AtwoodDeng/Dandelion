using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Grass : MonoBehaviour {

	[SerializeField] Transform[] parts;
	[SerializeField] MaxMin partscaleRange;
	[SerializeField] float growTime;

	void Start()
	{
		StartCoroutine( GrowAfterSetUp());
	}

	IEnumerator GrowAfterSetUp()
	{

		float stemRotateAngle = Random.Range( -5f , 5f );
		float totalGrowTime = 0 ;
		for(int i = 0 ; i < parts.Length ; i++)
		{
			Transform s = parts[i];

			// set the scale(start from V3(1,0,1))
			Vector3 scale = s.localScale;
			float scaleY = 1f;
			if (i != 0 ) scaleY = Random.Range(partscaleRange.min, partscaleRange.max);
			scale.y *= 0.001f * scaleY;
			s.localScale = scale;

			// set the rotation
			// each of the stem would rotate a little bit
			s.Rotate(new Vector3(0,0, stemRotateAngle));


			// set the grow animation
			float myGrowTime = growTime/parts.Length * scaleY / 0.7f / LogicManager.AnimTimeRate;
			if (i == parts.Length - 1 )
				s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(totalGrowTime).SetEase(Ease.Linear);
			else
				s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(totalGrowTime).SetEase(Ease.Linear);

			// update the grow time
			totalGrowTime += myGrowTime ;

		}

		yield break;
	}

}
