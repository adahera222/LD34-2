using UnityEngine;
using System.Collections;

public class BoardPlayArea : MonoBehaviour 
{
	/// <summary>
	/// The template piece.
	/// </summary>
	public GameObject TemplatePiece;
	
	public enum PlayAreaState
	{
		Idle,
		PieceSelected
	}
	
	private PlayAreaState _playAreaState = PlayAreaState.Idle;
	
	
	private GameObject _selectedObject = null;
	
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
		switch( _playAreaState )
		{
			case PlayAreaState.Idle:
			{
				if( Input.GetMouseButtonDown( 0 ) )
				{	
					var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
					var rayHits = Physics.RaycastAll( ray, Mathf.Infinity, 1 << Layers.Piece );
					if( rayHits.Length > 0 )
					{
						var rayHit = rayHits[0];
						_selectedObject = rayHit.collider.gameObject;
						_playAreaState = PlayAreaState.PieceSelected;
					}
				}
			}
			break;
			
			case PlayAreaState.PieceSelected:
			{
				var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				var rayHits = Physics.RaycastAll( ray, Mathf.Infinity, 1 << Layers.Board );
				if( rayHits.Length > 0 )
				{
					var rayHit = rayHits[0];
					
					_selectedObject.transform.position = rayHit.point + new Vector3( 0.0f, 0.2f, 0.0f );
					if( Input.GetMouseButtonDown( 0 ) )
					{	
						_selectedObject.transform.position = rayHit.point;
						//var objectMover = _selectedObject.GetComponent< ObjectMover >();
						//objectMover.Move( rayHit.point, objectMover.transform.rotation, 2.0f, null );
						_playAreaState = PlayAreaState.Idle;
					}
				}
			}
			break;
		}
	}
}
