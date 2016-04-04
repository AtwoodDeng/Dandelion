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

	bool m_isEnded;
	public bool isEnded
	{
		get { 
			return m_isEnded;
		}
	}
		

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
		EventManager.Instance.RegistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower,BlowFlower);
		EventManager.Instance.RegistersEvent(EventDefine.GrowFinalFlower,GrowFinalFlower);
		EventManager.Instance.RegistersEvent(EventDefine.AllBlackEndLevel,AllBlackEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.AllBlackRetry,AllBlackRetry);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
		EventManager.Instance.UnregistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower,BlowFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFinalFlower,GrowFinalFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.AllBlackEndLevel,AllBlackEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.AllBlackRetry,AllBlackRetry);
	}

	void Start()
	{
		EventManager.Instance.PostEvent( EventDefine.BeginLevel );
		if ( LevelManager != null )
			m_swipeTime = LevelManager.GetBlowTime();
		GameObject bgm = GameObject.FindGameObjectWithTag("BGM");
		if ( bgm == null )
		{
			bgm = Instantiate( Resources.Load(Global.BGM_PATH) ) as GameObject;

		}
		if ( bgm != null )
		{
			DontDestroyOnLoad( bgm );
		}
	}

	void AllBlackEndLevel( Message msg )
	{
		Application.LoadLevel( Global.NextLevel());
	}

	void AllBlackRetry( Message msg )
	{
		Application.LoadLevel( Application.loadedLevel );
	}

	void Update()
	{
		if ( m_isEnded && !checkEnd )
		{
			EventManager.Instance.PostEvent( EventDefine.EndLevel );
			checkEnd = true;
		}

		if ( Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftControl))
		{
			EventManager.Instance.PostEvent(EventDefine.EndLevel);
		}
	}

	bool checkEnd = false;
	void GrowFinalFlower( Message msg )
	{
		if ( m_levelManager.CheckLevelFinished() )
		{
			m_isEnded = true;
		}
	}



	void BlowFlower( Message msg )
	{
		m_swipeTime --; 
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
