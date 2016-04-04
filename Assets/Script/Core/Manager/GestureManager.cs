using UnityEngine;
using System.Collections;

public class GestureManager : MonoBehaviour {

	public GestureManager() { s_Instance = this; }
	public static GestureManager Instance { get { return s_Instance; } }
	private static GestureManager s_Instance;

	//blow the patels when swipe
	void OnSwipe( SwipeGesture gesture )
    {
        // if (swipeTime <= 0 )
        //    return;
        // swipeTime = swipeTime - 1;
        
		// make sure we started the swipe gesture on our swipe object
		GameObject selection = gesture.StartSelection;  // we use the object the swipe started on, instead of the current one
        
		if (selection == null )
            return;

		if ( LogicManager.Instance.RemainBlowTime > 0 )
		{
			SenseGuesture sense = selection.GetComponent<SenseGuesture>();

			if ( sense != null )
				sense.DealSwipe( gesture );
			
		}

    }

	void OnTap( TapGesture gesture )
	{
		Debug.Log("On Tap");
		// make sure we started the swipe gesture on our swipe object
		GameObject selection = gesture.StartSelection;  // we use the object the swipe started on, instead of the current one

		if (selection == null )
			return;

		Debug.Log("on select " + selection.name);
		SenseGuesture sense = selection.GetComponent<SenseGuesture>();
		if ( sense != null )
			sense.DealTap( gesture );
		
	}



}
