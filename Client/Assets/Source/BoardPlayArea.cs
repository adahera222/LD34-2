using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardPlayArea : MonoBehaviour 
{
	public BoardPiece TemplatePiece;
	public BoardPiecePlayLocation TemplatePiecePlayLocation;
	public List<BoardPiece> TemplateBoardPieces;
	
	public enum PlayAreaState
	{
		Idle
	}
		
	public int Size = 7;
	
	private PlayAreaState _playAreaState = PlayAreaState.Idle;
	
	// Pieces in pile to select from.
	private List<BoardPiece> _boardPiecePile = new List<BoardPiece>();
	
	// Played pieces.
	private BoardPiece[][] _boardPieceField = null;
	
	private Quaternion _pileRotation = Quaternion.Euler( new Vector3( 180.0f, 0.0f, 0.0f ) );
	
	// Use this for initialization
	void Start ()
	{
		// Create board pieces.
		foreach( var templateBoardPiece in TemplateBoardPieces)
		{
			for( int i = 0; i < templateBoardPiece.TotalInDeck; ++i )
			{
				var boardPieceObject = Object.Instantiate( templateBoardPiece.gameObject ) as GameObject;
				var boardPiece = boardPieceObject.GetComponent< BoardPiece >();
				_boardPiecePile.Add( boardPiece );
				boardPiece.SetupPiece( this, i );
			
				var position = GetPilePosition();
				boardPiece.transform.parent = transform;
				boardPiece.transform.localPosition = position;			
				boardPiece.transform.localRotation = _pileRotation;
			}
		}
		
		// Shuffle pieces.
		Random.seed = 0; // hard code seed for now.
		for( int i = 0; i < _boardPiecePile.Count * 4; ++i )
		{
			var randomIdx = Random.Range( 0, _boardPiecePile.Count);
			_boardPiecePile.Insert( randomIdx, _boardPiecePile[0] );
			_boardPiecePile.RemoveAt(0);
		}
		
		// Create board piece play locations.
		for( int y = -1; y <= Size; ++y )
		{
			for( int x = -1; x <= Size; ++x )
			{
				if( ( ( x < 0 || x >= Size ) ||
					  ( y < 0 || y >= Size ) ) &&
					( ( x >= 0 && x < Size ) ||
					  ( y >= 0 && y < Size ) ) &&
					( ( x != 3 && y != 3 ) ) )
				{
					var boardPieceObject = Object.Instantiate( TemplatePiecePlayLocation.gameObject ) as GameObject;
					var boardPiece = boardPieceObject.GetComponent< BoardPiecePlayLocation >();
			
					boardPiece.X = x;
					boardPiece.Y = y;
					boardPiece.transform.parent = transform;
					boardPiece.transform.localPosition = GetPiecePosition( x, y );
				}
			}
		}
		
		// Create board field.
		_boardPieceField = new BoardPiece[Size][];
		for( int i = 0; i < Size; ++i )
		{
			_boardPieceField[i] = new BoardPiece[Size];
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
					var rayHits = Physics.RaycastAll( ray, Mathf.Infinity, 1 << Layers.PiecePlayLocation );
					if( rayHits.Length > 0 )
					{
						var rayHit = rayHits[0];
					
						var boardPiece = rayHit.collider.gameObject.GetComponent< BoardPiecePlayLocation >();
						PlayTopPiece( boardPiece.X, boardPiece.Y );
					}
				}
			}
			break;
			
		}
	}
	
	// Get piece position.
	Vector3 GetPiecePosition(int x, int y)
	{
		// Centre x + y.
		x = x - ( Size / 2 );
		y = y - ( Size / 2 );
		
		return new Vector3((float)x, 0.0f, (float)y);
	}
	
	Vector3 GetPilePosition()
	{
		return GetPiecePosition( -1, Size / 2 );
	}
		
	// Play top piece to somewhere.
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
		else if( ( x >= 0 && x < Size ) ||
			     ( y >= 0 && y < Size ) )
		{
			x = Mathf.Clamp ( x, -1, Size );
			y = Mathf.Clamp ( y, -1, Size );
			
			BoardPiece boardPieceObject;
			Vector3 position;
			ObjectMover objectMover;

			if( x == -1 ) // Left
			{
				for( int i = Size - 1; i >= 0; --i )
				{
					boardPieceObject = _boardPieceField[i][y];
					_boardPieceField[i][y] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i + 1 ) < Size )
					{
						_boardPieceField[i + 1][y] = boardPieceObject;

						position = GetPiecePosition(i + 1, y);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, 0.5f, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, 2.0f, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[0][y] = boardPieceObject;
				position = GetPiecePosition(0, y);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, 1.0f, null);
			}
			else if( x == Size ) // Right
			{
				for( int i = 0; i < Size; ++i )
				{
					boardPieceObject = _boardPieceField[i][y];
					_boardPieceField[i][y] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i - 1 ) >= 0 )
					{
						_boardPieceField[i - 1][y] = boardPieceObject;

						position = GetPiecePosition(i - 1, y);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, 0.5f, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, 2.0f, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[Size - 1][y] = boardPieceObject;
				position = GetPiecePosition(Size - 1, y);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, 1.0f, null);
			}
			else if( y == -1 ) // Bottom
			{
				for( int i = Size - 1; i >= 0; --i )
				{
					boardPieceObject = _boardPieceField[x][i];
					_boardPieceField[x][i] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i + 1 ) < Size )
					{
						_boardPieceField[x][i + 1] = boardPieceObject;

						position = GetPiecePosition(x, i + 1);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, 0.5f, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, 2.0f, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[x][0] = boardPieceObject;
				position = GetPiecePosition(x, 0);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, 1.0f, null);
			}
			else if( y == Size ) // Top
			{
				for( int i = 0; i < Size; ++i )
				{
					boardPieceObject = _boardPieceField[x][i];
					_boardPieceField[x][i] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i - 1 ) >= 0 )
					{
						_boardPieceField[x][i - 1] = boardPieceObject;

						position = GetPiecePosition(x, i - 1);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, 0.5f, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, 2.0f, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[x][Size - 1] = boardPieceObject;
				position = GetPiecePosition(x, Size - 1);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, 1.0f, null);

			}
			
		}
	}
}
