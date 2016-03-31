using UnityEngine;
using System.Collections;

public class LogicManager : MonoBehaviour {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;


	static LevelManager m_levelManager;
	static public LevelManager LevelManager
	{
		get {
			if ( m_levelManager != null )
				return m_levelManager;

			if ( Instance != null )
				m_levelManager = Instance.GetComponent<LevelManager>();

			if ( m_levelManager == null && Level != null )
			{
				m_levelManager = Level.GetComponent<LevelManager>();
			}

			return m_levelManager;
		}
	}

	
	static GameObject m_level;
	static public GameObject Level
	{
		get {
			if ( m_level != null )
				return m_level;
			
			if ( m_levelManager != null )
				m_level = m_levelManager.GetLevelObject();

			if ( m_level == null )
				m_level = GameObject.Find("level");
				
			return m_level;
		}
	}

	int m_swipeTime = 3;
	public int RemainBlowTime
	{
		get { return m_swipeTime; }
	}
		
	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
		EventManager.Instance.RegistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower,BlowFlower);
		EventManager.Instance.RegistersEvent(EventDefine.GrowFinalFlower,GrowFinalFlower);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
		EventManager.Instance.UnregistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower,BlowFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFinalFlower,GrowFinalFlower);
	}

	void GrowFinalFlower( Message msg )
	{
		if ( m_levelManager.CheckLevelFinished() )
		{
			EventManager.Instance.PostEvent( EventDefine.EndLevel );
		}
	}

	void BlowFlower( Message msg )
	{
		m_swipeTime --; 
	}

	void Start()
	{
		if ( LevelManager != null )
			m_swipeTime = LevelManager.GetBlowTime();
	}

		
	void RenewSwipeTime(Message msg)
	{
		int time = (int)msg.GetMessage("time");
		m_swipeTime = time;
	}

	void AddSwipeTime(Message msg)
	{
		m_swipeTime ++;
	}
}
