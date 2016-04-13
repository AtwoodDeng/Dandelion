using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class OnEventFade : OnEvent {

	[SerializeField] float fadeTo;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] Image img;
	[SerializeField] bool isFrom;

	protected override void Do (Message msg)
	{
		if ( sprite == null )
		{
			sprite = GetComponent<SpriteRenderer>();
		}
		if ( sprite != null )
		{
			if ( isFrom)
				sprite.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).From();
			else
				sprite.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType);
		}


		if ( img == null )
		{
			img = GetComponent<Image>();
		}
		if ( img != null )
		{
			if ( isFrom )
				img.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).From();
			else
				img.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType);
		}

	}
}
