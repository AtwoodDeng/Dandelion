using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FunctionWindows : MonoBehaviour {

	[SerializeField] Text NumberText;

	void Update()
	{
		NumberText.text = LogicManager.Instance.RemainBlowTime.ToString();
	}

	float windButtonTime;
	public void OnWindButton()
	{
		if ( Time.time -  windButtonTime < 2f )
			return;
		
		WindAdv wind = LogicManager.LevelManager.GetWind();
		if ( wind.UIShowed )
			wind.HideUI();
		else
			wind.ShowUI();
		windButtonTime = Time.time;
	}

	public void OnRetryButton()
	{
		EventManager.Instance.PostEvent(EventDefine.Retry );
	}

	public void OnButtonEnd()
	{
		EventManager.Instance.PostEvent(EventDefine.EndLevel);
	}
}
