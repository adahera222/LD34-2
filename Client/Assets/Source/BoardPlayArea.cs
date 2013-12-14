using UnityEngine;
using System.Collections;

public class BoardPlayArea : MonoBehaviour 
{
	/// <summary>
	/// The template piece.
	/// </summary>
	public GameObject TemplatePiece;
	
	
	
	private 
	

	// Use this for initialization
	void Start ()
	{
		// Clone all board pieces.
		for( int Y = -3; Y <= 3; ++Y )
		{
			for( int X = -3; X <= 3; ++X )
			{
				var newPiece = Object.Instantiate( TemplatePiece ) as GameObject;
				var boardPiece = newPiece.GetComponent< BoardPiece >();
				boardPiece.SetupPiece( this, X, Y );
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
