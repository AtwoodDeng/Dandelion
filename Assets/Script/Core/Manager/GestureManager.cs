using UnityEngine;
using System.Collections;

public class GestureManager : MonoBehaviour {

	public GestureManager() { s_Instance = this; }
	public static GestureManager Instance { get { return s_Instance; } }
	private static GestureManager s_Instance;

    [SerializeField] int swipeTime;

	//blow the patels when swipe
	void OnSwipe( SwipeGesture gesture )
    {
        Debug.Log("On swipe");
        if (swipeTime <= 0 )
            return;
        swipeTime = swipeTime - 1;
        // make sure we started the swipe gesture on our swipe object
        GameObject selection = gesture.StartSelection;  // we use the object the swipe started on, instead of the current one
        
        if (selection == null)
             Debug.Log("select nothing");

        Debug.Log("select "+ selection.name);
        if (selection == null )
            return;
        Flower flowerComp = selection.GetComponent<Flower>();

        if (flowerComp != null )
        	flowerComp.Blow(gesture.Move,gesture.Velocity);

    }


    void OnEnable()
    {
        EventManager.Instance.RegistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
        EventManager.Instance.RegistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
    }

    void OnDisable()
    {
        EventManager.Instance.UnregistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
        EventManager.Instance.UnregistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);

    }

    void RenewSwipeTime(Message msg)
    {
        int time = (int)msg.GetMessage("time");
        swipeTime = time;
    }

    void AddSwipeTime(Message msg)
    {
        swipeTime ++;
    }

    void OnGUI()
    {
        GUILayout.TextField("remaining swipe time : " + swipeTime.ToString() );
    }

}
