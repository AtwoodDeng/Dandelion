﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Flower3D : Flower , WindSensable {

	[SerializeField] Transform[] stems;
	[SerializeField] GameObject LeafPrefab;
	[SerializeField] GameObject FlowerPetalPrefab;
	[SerializeField] GameObject FlowerPetalFinalPrefab;
	[SerializeField] float growDelay = 1f; 
	[SerializeField] Petal3D[] flowerPetals;

	List<SpriteRenderer> leafList = new List<SpriteRenderer>();

	[SerializeField] protected Grow3DParameter grow3DPara;

	[System.SerializableAttribute]
	public struct Grow3DParameter
	{
		public  MaxMin stemScaleRange;
		public  MaxMin stemRotateAngleRange;
        public  float stemWindAffect;

		public  MaxMin leafScaleRange;
		public  MaxMin leafRotateAngleRange;
		public  MaxMin leafPositionYRange;
        public MaxMin leafNumberRange;

        public float stemWindSwind;

		public int flowerPetalNumber;
		public float flowerPetalUnlinkInterval;
	};

    // public override void Init() {
    //     base.Init();

    //     Petal[] ps = petalRoot.GetComponentsInChildren<Petal3D>();
    //     petals.AddRange(ps);
    //     foreach(Petal p in petals)
    //     {
    //         p.gameObject.SetActive(false);
    //         p.state = PetalState.Link;
    //     }
    // }

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
	}

	void OnEndLevel(Message msg )
	{
		for( int i = 0 ; i < petals.Count ; ++ i)
		{
			petals[i].Blow( Global.GetRandomDirection() , 0 , Petal.BlowType.FlyAway );
		}
	}



	public override void Grow () {

        StartCoroutine(GrowAfterSetUp());
    }

    IEnumerator GrowAfterSetUp()
    {
        transform.localScale /= 1000f;
        float timer = 0;
		while( timer < Mathf.Min(0.1f , growDelay) )
        {
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale *= 1000f;
        // init the stems
		// to record the grow time of stem
		float stemGrowTime = 0;
		//TODO set the angle according to wind
		float stemRotateAngle = Random.Range( grow3DPara.stemRotateAngleRange.min , grow3DPara.stemRotateAngleRange.max);

        if (windVelocity != Vector2.zero)
        {
         stemRotateAngle = Vector2.Angle(Vector2.up, Vector2.up + grow3DPara.stemWindAffect * windVelocity) * Vector2.Dot( Vector2.left , windVelocity)
         + Random.Range( grow3DPara.stemRotateAngleRange.min , grow3DPara.stemRotateAngleRange.max);
    	} 
        for(int i = 0 ; i < stems.Length ; i++)
    	{
    		Transform s = stems[i];

			if ( growParameter.petalType == PetalType.Final )
			{
				SpriteRenderer sprite = stems[i].GetComponent<SpriteRenderer>();
				Material m = new Material( sprite.material.shader );
				Debug.Log(m.name );
				Color fromColor = Color.Lerp( Color.black , Color.white , 1.0f * i / stems.Length );
				Color toColor = Color.Lerp( Color.black , Color.white , 1.0f * ( i + 1 ) / stems.Length );
				m.SetColor("_ColorFirst" , fromColor );
				m.SetColor("_ColorSecond" , toColor );
				m.SetFloat("_ChangeRate" , 0.5f );
				m.SetFloat("_ChangeRange" , 1.0f );
				m.name = "Stem" + i;
				sprite.material = m;
//				Color col = Color.white;
//				col.a = sprite.color.a;
//				sprite.color = col;
//				sprite.color = Color.white;
			}

            // set the scale(start from V3(1,0,1))
    		Vector3 scale = s.localScale;
    		float scaleY = 1f;
            if (i != 0 ) scaleY = Random.Range(grow3DPara.stemScaleRange.min, grow3DPara.stemScaleRange.max);
    		scale.y *= 0.001f * scaleY;
            s.localScale = scale;

            // set the rotation
            // each of the stem would rotate a little bit
    		s.Rotate(new Vector3(0,0, stemRotateAngle));


            // set the grow animation
    		float myGrowTime = growParameter.growTime/stems.Length * scaleY / 0.7f;
    		if (i == stems.Length - 1 )
	    		s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(stemGrowTime).SetEase(Ease.Linear).OnComplete(GrowStemComplete);
			else
				s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(stemGrowTime).SetEase(Ease.Linear);

            // update the grow time
            stemGrowTime += myGrowTime;

    	}

        // init the leafs
    	int leafs = Random.Range( (int)grow3DPara.leafNumberRange.min, (int)grow3DPara.leafNumberRange.max);
    	for( int k = 0 ; k < leafs; ++k )
    	{
            // create the leaf
    		GameObject leaf = Instantiate(LeafPrefab) as GameObject;
    		SpriteRenderer sprite = leaf.GetComponent<SpriteRenderer>();

    		leaf.transform.parent = stems[0];
    		leaf.transform.localPosition = new Vector3(0,Random.Range(grow3DPara.leafPositionYRange.min, grow3DPara.leafPositionYRange.max),0);
    		
            // set the scale(a random value)
            float scale = Random.Range(grow3DPara.leafScaleRange.min, grow3DPara.leafScaleRange.max) ;
    		leaf.transform.localScale = Vector3.one * scale / stems[0].localScale.x;
            
            // set the rotation ( a random value)
            float angleRange = ( - grow3DPara.leafRotateAngleRange.min + grow3DPara.leafRotateAngleRange.max) ;
            float randomeAngle = grow3DPara.leafRotateAngleRange.min + k * angleRange / leafs  + Random.Range(0, angleRange / (leafs-1) );
    		leaf.transform.Rotate(new Vector3(0,0, randomeAngle));
    		
    		// set the scale animation
    		// make all the leaf grow in same speed
    		// and make the bigger leaf grow earlier
 			float process = (scale - grow3DPara.leafScaleRange.min + 0.2f) / ( grow3DPara.leafScaleRange.max - grow3DPara.leafScaleRange.min);
    		float growTime = process * growParameter.growTime / grow3DPara.leafScaleRange.max;
            growTime = Mathf.Clamp(growTime, 0.5f, 3f);
    		leaf.transform.DOScale( 0.01f , growTime  )
    		.SetDelay( (1 - scale / grow3DPara.leafScaleRange.max ) * growParameter.growTime )
    		.From();
            sprite.DOFade(0, growTime / 2).SetDelay( (1 - scale / grow3DPara.leafScaleRange.max ) * growParameter.growTime ).From();
    	}
    }

    void GrowStemComplete()
    {
		Vector3 rootLS = petalRoot.transform.localScale;
		Vector3 rootGS = petalRoot.transform.lossyScale;
		petalRoot.transform.localScale = new Vector3( rootLS.x / rootGS.x , rootLS.y / rootGS.y , rootLS.z / rootGS.z);


        BoxCollider boxCol = GetComponent<BoxCollider>();
        boxCol.center = petalRoot.position - transform.position;

		GrowFlowerPatel();
    }

	void GrowFlowerPatel()
	{
		StartCoroutine(GrowFlowerPatalCor(grow3DPara.flowerPetalNumber));
	}


	IEnumerator GrowFlowerPatalCor(int num)
	{
		flowerPetals = new Petal3D[num];

		float maxGrowTime = 0.5f ;
		for(int i = 0 ; i < num ; ++ i )
		{
			GameObject prefab = (growParameter.petalType == PetalType.Final )? FlowerPetalFinalPrefab : FlowerPetalPrefab;
			GameObject fPetalObj = Instantiate( prefab ) as GameObject;
			fPetalObj.transform.SetParent( petalRoot );
			fPetalObj.transform.localPosition = Vector3.zero;
			fPetalObj.transform.localScale = Vector3.one;

			flowerPetals[i] = fPetalObj.GetComponent<Petal3D>();
			if ( flowerPetals[i] != null )
			{
				flowerPetals[i].Init( this , i ) ;
			}
				
			maxGrowTime = Mathf.Max( maxGrowTime , flowerPetals[i].GetGrowTime() );
		}
		yield return new WaitForSeconds(maxGrowTime);

		if ( growParameter.petalType == PetalType.Final ) 
		{
			Message msg = new Message();
			msg.AddMessage( "flower" , this );

			EventManager.Instance.PostEvent(EventDefine.GrowFinalFlower , msg );
			yield break;
		}else
		{
			for ( int i = 0 ; i < flowerPetals.Length ; ++ i )
			{
				flowerPetals[i].Blow( Global.GetRandomDirection() , 0 , Petal.BlowType.FlyAway );
				yield return new WaitForSeconds(grow3DPara.flowerPetalUnlinkInterval );
			}

			StartCoroutine(GrowPetalCor());
		}
			
	}

	public void BlowFlowerPetals()
	{
		StartCoroutine(BlowFlowerPetalsCor());
	}

	IEnumerator BlowFlowerPetalsCor()
	{
		for ( int i = 0 ; i < flowerPetals.Length ; ++ i )
		{
			flowerPetals[i].Blow( Global.GetRandomDirection() , 0 , Petal.BlowType.FlyAway );
			yield return new WaitForSeconds(grow3DPara.flowerPetalUnlinkInterval );
		}
		yield break;
	}

	protected override bool canBlow ()
	{
		if ( petals.Count < petalNum - 1 )
			return false;
		Petal petal = petals[0];
		if ( petal != null && Time.time - initPetalTime < petal.GetGrowTime())
			return false;
		return base.canBlow();
	}

	public override void Blow (Vector2 move, float velocity)
	{
		if ( canBlow() )
		{
			base.Blow (move, velocity);

			// make the flower react to the blow
			FollowWind stemFollowWind = stems[0].GetComponent<FollowWind>();
			if ( stemFollowWind != null )
			{
				stemFollowWind.AddImpuse( move * velocity , petalRoot.position );
			}
		}
	}

	public int GetFlowerNum()
	{
		return grow3DPara.flowerPetalNumber;
	}


    Vector2 windVelocity;

    public void SenseWind(Vector2 velocity)
    {
        windVelocity = velocity;
    }

    public void ExitWind(){
        windVelocity = Vector2.zero;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    [SerializeField] WindSensablParameter windSensablParameter;

    public WindSensablParameter getParameter()
    {
        return windSensablParameter;
    }

    void OnBecameVisible()
    {
        windSensablParameter.shouldUpdate = true;
        enabled = true;
    }

    void OnBecameInvisible ()
    {
        windSensablParameter.shouldUpdate = false;
        enabled = false;
    }
}
