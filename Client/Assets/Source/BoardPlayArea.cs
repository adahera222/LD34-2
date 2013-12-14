using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	public int Size = 7;
	
	private PlayAreaState _playAreaState = PlayAreaState.Idle;
	private GameObject _selectedObject = null;
	
	// Pieces in pile to select from.
	private List<GameObject> _boardPiecePile = new List<GameObject>();
	
	// Played pieces.
	private GameObject[][] _boardPieceField = null;
	
	// Get piece position.
	Vector3 GetPiecePosition( int x, int y )
	{
		// Centre x + y.
		x = x - ( Size / 2 );
		y = y - ( Size / 2 );
		
		return new Vector3( (float)x, 0.0f, (float)y );
	}
		
	// Use this for initialization
	void Start ()
	{
		// Create board pieces.
		for( int i = 0; i < 60; ++i )
		{
			var boardPieceObject = Object.Instantiate( TemplatePiece ) as GameObject;
			var boardPiece = boardPieceObject.GetComponent< BoardPiece >();
			_boardPiecePile.Add( boardPieceObject );
			boardPiece.SetupPiece( this, i );
			
			var position = GetPiecePosition( -1, Size / 2 );
			boardPiece.transform.localPosition = position;			
		}
		
		// Create board field.
		_boardPieceField = new GameObject[Size][];
		for( int i = 0; i < Size; ++i )
		{
			_boardPieceField[i] = new GameObject[Size];
		}
		
		// Play pieces.
		for( int y = 0; y < 7; ++y )
		{
			for( int x = 0; x < 7; ++x )
			{
				PlayTopPiece( x, y );
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
	
	public void PlayTopPiece(int x, int y)
	{
		// Grab and move between lists.
		if( x >= 0 && x < Size &&
			y >= 0 && y < Size )
		{
			var boardPieceObject = _boardPiecePile[0];
			_boardPiecePile.RemoveAt (0);
			_boardPieceField[x][y] = boardPieceObject;
			
			// Move to X & Y.
			var position = GetPiecePosition(x, y);
			var objectMover = boardPieceObject.GetComponent< ObjectMover >();
			objectMover.Move(position, Quaternion.identity, 1.0f, null);
		}
	}
}
