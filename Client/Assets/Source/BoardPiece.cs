using UnityEngine;
using System.Collections;

public class BoardPiece : MonoBehaviour
{
	private int _id = 0;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	// Setup piece with play area and position.
	public void SetupPiece( BoardPlayArea playArea, int id )
	{
		// Parent transform.
		if( playArea != null )
		{
			transform.parent = playArea.transform;
		}
		
		// ID.
		if( id != -1 )
		{
			_id = id;
		}
			
		// Setup name for easy debugging.
		this.gameObject.name = string.Format( "BoardPiece({0})", _id );
	}
	
	
}
