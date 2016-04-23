using UnityEngine;
using System.Collections;

public class Land : MonoBehaviour {
	[SerializeField] float PosLayer = 0 ;
	float m_layer;
	Vector3 oriPosition;
//
//	[SerializeField] ComputeShader computeShader;
//	[SerializeField] Shader fadeOutShader;
//
//	[SerializeField] Texture2D InkTexture;
//	[SerializeField] Texture2D resultTexture;
//
//	Material m_Material;
//
//	static int ThreedNum = 32;
//	int m_nNumGroupsX = 1;
//	int m_nNumGroupsY = 1;
//	int kernelIndex = 0;
//
//	ComputeBuffer[] m_computeBuffer;
//	Texture m_Texture;
//	Texture tem_Texture;
//
//	void Awake()
//	{
//		m_Material = new Material(fadeOutShader);
//		m_Texture = new Texture2D( resultTexture.width , resultTexture.height );
//		tem_Texture = new Texture2D( resultTexture.width , resultTexture.height );
//
//		GetComponent<SpriteRenderer>().material = m_Material;
//		m_Material.mainTexture = m_Texture;
//
//		m_nNumGroupsX = resultTexture.width / ThreedNum;
//		m_nNumGroupsY = resultTexture.height / ThreedNum;
//	}
//
//	void Update()
//	{
//		m_Material.SetTexture("_MainTex" , m_Texture);
//	}
	[SerializeField] tk2dSprite thisSprite;
	[SerializeField] SpriteRenderer effectSprite;

	[System.Serializable]
	public struct EffectParameter
	{
		public Shader effectShader;
		public Texture2D resultTex;
		public Texture2D coverTex;
		public Color mainColor;
		public Vector4 coverInit;
		public Vector4 CoverMove;
		public float fadePosition;
		public float fadeRange;
		public AnimationCurve sizeCurve;
		public MaxMin delay;
		public MaxMin expand;
		public int dropNum;
		public float growTime;
	}

	[SerializeField] EffectParameter effectParameter;
	Texture m_texture;
	Material m_material;


	public static int CoverRecordNum = 10;
	Vector4[] CoverRec = new Vector4[CoverRecordNum];

	public struct CoverInfo
	{
		public Vector4 to;
		public Vector4 tem;
		public float process;
		public float delay;
	}

	CoverInfo[] coverInfos = new CoverInfo[CoverRecordNum];
//	Vector4[] coverInitRec = new Vector4[CoverRecordNum];
//	Vector4[] coverTemRec = new Vector4[CoverRecordNum];
//	float coverDelay;

	void Awake()
	{
		oriPosition = transform.localPosition;

		m_layer = (PosLayer) * 0.1f;

		if ( thisSprite == null )
			thisSprite = GetComponentInChildren<tk2dSprite>();
		if ( GetComponent<MeshRenderer>() != null )
			GetComponent<MeshRenderer>().enabled = false;
		
		if ( effectSprite == null )
			effectSprite = GetComponentInChildren<SpriteRenderer>();
		
		if ( effectParameter.resultTex == null && effectSprite != null )
			effectParameter.resultTex = (Texture2D) effectSprite.material.mainTexture;

		if (effectParameter.effectShader != null )
			m_material = new Material(effectParameter.effectShader);
		if ( effectSprite != null )
			effectSprite.material = m_material;
		if ( m_material != null )
		{
			m_material.SetTexture("_MainTex" , effectParameter.resultTex );
			m_material.SetTexture("_CoverTex" , effectParameter.coverTex );
			m_material.SetColor("_Color" , effectParameter.mainColor );
			m_material.SetFloat("_FadePos" , effectParameter.fadePosition );
			m_material.SetFloat("_FadeRange" , effectParameter.fadeRange );
			m_material.SetInt("_CountNum" , effectParameter.dropNum );

			for( int i = 0 ; i < CoverRecordNum ; ++ i )
			{
				Vector4 toVec =  new Vector4();

				if ( i == 0 )
				{
					toVec.x = toVec.y = 0;
				}else
				{
					toVec.x = Random.Range( - effectParameter.coverInit.x , effectParameter.coverInit.x);
					toVec.y = Random.Range( - effectParameter.coverInit.y , effectParameter.coverInit.y);
				}
				toVec.z = 1.0f / effectParameter.coverTex.width * effectParameter.resultTex.width ;
				toVec.w = 1.0f / effectParameter.coverTex.height * effectParameter.resultTex.height;

				coverInfos[i].tem = toVec;
				coverInfos[i].delay = Random.Range(effectParameter.delay.min,effectParameter.delay.max);

				float temScale = Random.Range( effectParameter.expand.min , effectParameter.expand.max);
				toVec.z /= temScale;
				toVec.w /= temScale;

				coverInfos[i].to = toVec;
				coverInfos[i].process = 0;

				toVec.x = 9999f;
				toVec.y = 9999f;

				m_material.SetVector( "_CoverRec" + i.ToString() , toVec );
			}

//			// set Gauss Value
//			for( int i = 0 ; i < Global.GaussSize ; ++ i )
//				for( int j = 0 ; j < Global.GaussSize ; ++ j )
//				{
//					int index = i * Global.GaussSize + j ;
//					m_material.SetFloat( "_GaussValue" + index.ToString() , GetGaussValue( i , j )  );
//					Debug.Log("Compare" + GetGaussValue(i,j) + " " + Global.GaussValue[index] );
//				}


//			coverTemRec.x = effectParameter.coverInit.x;
//			coverTemRec.y = effectParameter.coverInit.y;
//			coverTemRec.z = 1.0f / effectParameter.coverTex.width * effectParameter.resultTex.width;
//			coverTemRec.w = 1.0f / effectParameter.coverTex.height * effectParameter.resultTex.height;
		}
		if ( CoverRec == null)
			CoverRec = new Vector4[CoverRecordNum];
		for( int i = 0 ; i < CoverRecordNum ; ++ i )
		{
			CoverRec[i] = new Vector4( 0 , 0 , 1f , 1f );
		}

		//TODO : FOR TEST
//		effectSprite.enabled = false;
		
		// m_texture = new Texture2D( result.width , result.height);
	}

	float GetGaussValue( int i , int j )
	{
		float x = i - (Global.GaussSize - 1 ) * 0.5f;
		float y = j - (Global.GaussSize - 1 ) * 0.5f;
		float res = 1 / ( 2f * Mathf.PI * Global.GaussSigma * Global.GaussSigma );
		res *= Mathf.Exp( - ( x * x + y * y ) / ( 2f * Global.GaussSigma * Global.GaussSigma ) );
		return res ;
	}

	void UpdatePosition()
	{
		transform.localPosition = oriPosition + LogicManager.CameraManager.OffsetFromInit * ( m_layer );
	}

	float timer = 0 ;
	bool isGrowFinished = false;
	void UpdateSprite()
	{
		timer += Time.deltaTime;
		if ( !isGrowFinished )
		for ( int i = 0 ; i < 3 ; ++ i )
		{
			if ( timer > coverInfos[i].delay )
			{
				coverInfos[i].process += Time.deltaTime / effectParameter.growTime;
				coverInfos[i].tem.z = coverInfos[i].to.z / effectParameter.sizeCurve.Evaluate( coverInfos[i].process );
				coverInfos[i].tem.w = coverInfos[i].to.w / effectParameter.sizeCurve.Evaluate( coverInfos[i].process );

				m_material.SetVector( "_CoverRec" + i.ToString() , coverInfos[i].tem );
			}
		}


		if ( timer > effectParameter.growTime && !isGrowFinished )
		{
			FinishGrow();
		}
	}

	void FinishGrow()
	{
		effectSprite.enabled = false;
//		Color col = thisSprite.color;
//		col.a = 1f;
//		thisSprite.color = col;
		thisSprite.color = effectParameter.mainColor;
		isGrowFinished = true;

		if ( GetComponent<MeshRenderer>() != null )
			GetComponent<MeshRenderer>().enabled = true;
	}

	void Update()
	{
		UpdatePosition();
		UpdateSprite();
	}
}
