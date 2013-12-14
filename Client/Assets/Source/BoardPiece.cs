using UnityEngine;
using System.Collections;

public class BoardPiece : MonoBehaviour
{
	private int _x = 0;
	private int _y = 0;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	// Setup piece with play area and position.
	public void SetupPiece( BoardPlayArea playArea, int X, int Y )
	{
		// Parent transform.
		if( playArea != null )
		{
			transform.parent = playArea.transform;
		}
		
		// X & Y.
		_x = X;
		_y = Y;
		
		// Setup position.
		transform.localPosition = new Vector3( (float)X, 0.0f, (float)Y );
		
		// Setup name for easy debugging.
		this.gameObject.name = string.Format( "BoardPiece({0},{1})", _x, _y );
	}
	
	
}
