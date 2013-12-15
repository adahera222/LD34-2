using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{
	
	private float _ticker = 0.0f;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		_ticker += Time.deltaTime;
		
		if( _ticker > Mathf.PI * 2.0f )
		{
			_ticker -= Mathf.PI * 2.0f;
		}
		
		transform.rotation = Quaternion.Euler( new Vector3( 0.0f, Mathf.Rad2Deg * _ticker, 0.0f ) );
	}
}
