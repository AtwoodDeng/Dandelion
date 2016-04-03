using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventEmit : OnEvent {

	[SerializeField] ParticleSystem ps;
	[SerializeField] float toRate;

	protected override void Do (Message msg)
	{
		if (ps == null )
		 ps = GetComponent<ParticleSystem>();
		if ( ps != null )
		{
			StartCoroutine(emit(ps));
		}
	}

	IEnumerator emit(ParticleSystem ps)
	{
		float timer = 0;
		yield return new WaitForSeconds( delay );
		ps.enableEmission = true;
		ps.Play();

		while( true )
		{
			if ( timer > time ) 
				yield break;
			
			timer += Time.deltaTime;
			ps.emissionRate = timer / time * toRate;


			yield return null;
		}
	}
}
