using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class WindGenerator : MonoBehaviour {

	[SerializeField] float strength;
	[SerializeField] Vector3 Size;
	[SerializeField] float direction;
	public bool isForce;


	void Awake()
	{
		GetComponent<BoxCollider>().size = Size;
		gameObject.layer = LayerMask.NameToLayer("WindGenerator");
	}
	public Vector3 GetDirection()
	{
		return new Vector3( Mathf.Cos(direction ) , Mathf.Sin(direction), 0 ).normalized;
	}

	public Vector2 GetVelocity( Vector3 pos )
	{
		return GetDirection() * strength;
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, Size);
		Gizmos.DrawLine(transform.position, transform.position + GetDirection() * strength * 0.1f );
	}
}
