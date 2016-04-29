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
			sprite.enabled = true;
			if ( isFrom)
				sprite.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).From().OnComplete(CheckTheAlpha);
			else
				sprite.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).OnComplete(CheckTheAlpha);
		}


		if ( img == null )
		{
			img = GetComponent<Image>();
		}
		if ( img != null )
		{
			img.enabled = true;
			if ( isFrom )
				img.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).From().OnComplete(CheckTheAlpha);
			else
				img.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).OnComplete(CheckTheAlpha);
		}

	}

	void CheckTheAlpha()
	{
		if ( sprite != null && sprite.color.a <= 0.01f )
			sprite.enabled = false;
		if ( img != null && img.color.a <= 0.01f )
			img.enabled = false;
	}
}
