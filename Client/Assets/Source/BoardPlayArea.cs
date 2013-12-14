using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardPlayArea : MonoBehaviour 
{
	public BoardPiecePlayLocation TemplatePiecePlayLocation;
	public List<BoardPiece> TemplateBoardPieces;
	public List<PlayerPiece> TemplatePlayerPieces;
	
	public enum PlayAreaState
	{
		PlaceNewTile,
		MovePlayer,
		NextTurn
	}
	
	public int Size = 7;	
	
	public float MoveHeightForTileSlide = 0.0f;
	public float MoveHeightForNewTile = 1.0f;
	public float MoveHeightForReturnTile = 2.0f;
	
	private PlayAreaState _playAreaState = PlayAreaState.PlaceNewTile;
	
	// Pieces in pile to select from.
	private List<BoardPiece> _boardPiecePile = new List<BoardPiece>();
	
	// Played pieces.
	private BoardPiece[][] _boardPieceField = null;
	
	// Player pieces.
	private List<PlayerPiece> _playerPieces = new List<PlayerPiece>();
	
	// Pile rotation.
	private Quaternion _pileRotation = Quaternion.Euler( new Vector3( 180.0f, 0.0f, 0.0f ) );
	
	// Active player index.
	private int _activePlayerIndex = 0;
	
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
		
		// Find start piece and remove.
		BoardPiece startPiece = null;
		for( int i = 0; i < _boardPiecePile.Count; ++i )
		{
			if( _boardPiecePile[ i ].IsStart )
			{
				startPiece = _boardPiecePile[ i ];
				_boardPiecePile.RemoveAt( i );
				break;
			}
		}
		
		startPiece.transform.localRotation = Quaternion.identity;
		
		// Create board piece play locations.
		int centre = Size / 2;
		for( int y = -1; y <= Size; ++y )
		{
			for( int x = -1; x <= Size; ++x )
			{
				if( ( ( x < 0 || x >= Size ) ||
					  ( y < 0 || y >= Size ) ) &&
					( ( x >= 0 && x < Size ) ||
					  ( y >= 0 && y < Size ) ) &&
					( ( x != centre && y != centre ) ) )
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
				if( x == centre && y == centre )
				{
					PlayPiece( x, y, startPiece );
				}
				else
				{
					PlayPiece( x, y, null );
				}
			}
		}
		
		// Flip over top piece for next move.
		{
			BoardPiece boardPiece = _boardPiecePile[0];
			var objectMover = boardPiece.GetComponent< ObjectMover >();
			objectMover.Move(GetPilePosition() + new Vector3( 1.0f, 0.0f, 0.0f ), Quaternion.identity, MoveHeightForNewTile, null);
		}
		
		// Create players.
		int noofPlayers = 4;
		for( int i = 0; i < noofPlayers; ++i )
		{
			var playerPieceObject = Object.Instantiate( TemplatePlayerPieces[i].gameObject ) as GameObject;
			var playerPiece = playerPieceObject.GetComponent< PlayerPiece >();
			
			var transformOnTile = GetTransformOnTile( centre, centre, i );
			if( transformOnTile != null )
			{
				playerPiece.transform.parent = _boardPieceField[centre][centre].transform;
				playerPiece.transform.position = transformOnTile.position;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch( _playAreaState )
		{
			case PlayAreaState.PlaceNewTile:
			{
				if( Input.GetMouseButtonDown( 0 ) )
				{
					var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
					var rayHits = Physics.RaycastAll( ray, Mathf.Infinity, 1 << Layers.PiecePlayLocation );
					if( rayHits.Length > 0 )
					{
						var rayHit = rayHits[0];
						
						var boardPiece = rayHit.collider.gameObject.GetComponent< BoardPiecePlayLocation >();
						PlayPiece( boardPiece.X, boardPiece.Y, null );
						
						_playAreaState = PlayAreaState.MovePlayer;
					}
				}
			}
			break;
			
			case PlayAreaState.MovePlayer:
			{
			}
			break;

			case PlayAreaState.NextTurn:
			{
				// Funky animation bro!
				_playAreaState = PlayAreaState.PlaceNewTile;
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
	
	Transform GetTransformOnTile(int x, int y, int edge)
	{
		return _boardPieceField[x][y].GetEdgePieceTransform(edge);
	}
	
	Vector3 GetPilePosition()
	{
		return GetPiecePosition( -2, Size / 2 );
	}
		
	// Play top piece to somewhere.
	public void PlayPiece(int x, int y, BoardPiece piece)
	{
		// Grab and move between lists.
		if( x >= 0 && x < Size &&
			y >= 0 && y < Size )
		{
			var boardPieceObject = piece == null ? _boardPiecePile[0] : piece;
			if( piece == null )
			{
				_boardPiecePile.RemoveAt (0);
			}
			boardPieceObject.Coord = new TileCoord(x, y);
			_boardPieceField[x][y] = boardPieceObject;
			
			// Move to X & Y.
			var position = GetPiecePosition(x, y);
			var objectMover = boardPieceObject.GetComponent< ObjectMover >();
			objectMover.Move(position, Quaternion.identity, MoveHeightForNewTile, null);
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
					boardPieceObject.Coord = new TileCoord(-1, -1);
					_boardPieceField[i][y] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i + 1 ) < Size )
					{
						_boardPieceField[i + 1][y] = boardPieceObject;
						boardPieceObject.Coord = new TileCoord(i + 1, y);

						position = GetPiecePosition(i + 1, y);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, MoveHeightForTileSlide, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, MoveHeightForReturnTile, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[0][y] = boardPieceObject;
				boardPieceObject.Coord = new TileCoord(0, y);
				position = GetPiecePosition(0, y);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, MoveHeightForNewTile, null);
			}
			else if( x == Size ) // Right
			{
				for( int i = 0; i < Size; ++i )
				{
					boardPieceObject = _boardPieceField[i][y];
					boardPieceObject.Coord = new TileCoord(-1, -1);
					_boardPieceField[i][y] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i - 1 ) >= 0 )
					{
						_boardPieceField[i - 1][y] = boardPieceObject;
						boardPieceObject.Coord = new TileCoord(i - 1, y);

						position = GetPiecePosition(i - 1, y);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, MoveHeightForTileSlide, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, MoveHeightForReturnTile, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[Size - 1][y] = boardPieceObject;
				boardPieceObject.Coord = new TileCoord(Size - 1, y);
				position = GetPiecePosition(Size - 1, y);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, MoveHeightForNewTile, null);
			}
			else if( y == -1 ) // Bottom
			{
				for( int i = Size - 1; i >= 0; --i )
				{
					boardPieceObject = _boardPieceField[x][i];
					boardPieceObject.Coord = new TileCoord(-1, -1);
					_boardPieceField[x][i] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i + 1 ) < Size )
					{
						_boardPieceField[x][i + 1] = boardPieceObject;
						boardPieceObject.Coord = new TileCoord(x, i + 1);

						position = GetPiecePosition(x, i + 1);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, 0.5f, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, MoveHeightForReturnTile, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[x][0] = boardPieceObject;
				boardPieceObject.Coord = new TileCoord(x, 0);
				position = GetPiecePosition(x, 0);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, MoveHeightForNewTile, null);
			}
			else if( y == Size ) // Top
			{
				for( int i = 0; i < Size; ++i )
				{
					boardPieceObject = _boardPieceField[x][i];
					boardPieceObject.Coord = new TileCoord(-1, -1);
					_boardPieceField[x][i] = null;
					
					// Handle moving to grid coord + back to pile.
					if( ( i - 1 ) >= 0 )
					{
						_boardPieceField[x][i - 1] = boardPieceObject;
						boardPieceObject.Coord = new TileCoord(x, i - 1);
						
						position = GetPiecePosition(x, i - 1);
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, Quaternion.identity, MoveHeightForTileSlide, null);
					}
					else
					{
						position = GetPilePosition();
						objectMover = boardPieceObject.GetComponent< ObjectMover >();
						objectMover.Move(position, _pileRotation, MoveHeightForReturnTile, null);

						_boardPiecePile.Add ( boardPieceObject );						
					}
				}
				
				// Add new piece.
				boardPieceObject = _boardPiecePile[0];
				_boardPiecePile.RemoveAt (0);
				_boardPieceField[x][Size - 1] = boardPieceObject;
				boardPieceObject.Coord = new TileCoord(x, Size - 1);
				position = GetPiecePosition(x, Size - 1);
				objectMover = boardPieceObject.GetComponent< ObjectMover >();
				objectMover.Move(position, Quaternion.identity, MoveHeightForNewTile, null);
			}
			
			// Flip over top piece for next move.
			boardPieceObject = _boardPiecePile[0];
			objectMover = boardPieceObject.GetComponent< ObjectMover >();
			objectMover.Move(GetPilePosition() + new Vector3( 1.0f, 0.0f, 0.0f ), Quaternion.identity, MoveHeightForNewTile, null);
		}
	}
	
	bool IsValidMove( TileCoord fromCoord, TileCoord targetCoord )
	{
		// Clear path finding data.
		for( int y = 0; y < Size; ++y )
		{
			for( int x = 0; x < Size; ++x )
			{
				_boardPieceField[x][y].ClearPathfinding();
			}
		}
		
		// Check neighbours.
		if( IsValidMoveCheckNeighbours( fromCoord, targetCoord, 0 ) )
		{
			// If we have found, work back from the target coord down the quickest path.
			List<TileCoord> path = new List<TileCoord>();
			TileCoord currCoord = targetCoord;
			TileCoord nextCoord = targetCoord;
			while( !currCoord.Equals( fromCoord ) )
			{
				path.Add ( currCoord );
				
				int lowestDistance = _boardPieceField[ currCoord.x ][ currCoord.y ].TentativeDistance;
				
				for( int i = 0; i < 4; ++i )
				{
					if( IsValidMoveSingleStep( currCoord, i ) )
					{
						var testCoord = currCoord.Next( i );
						int distance = _boardPieceField[ testCoord.x ][ testCoord.y ].TentativeDistance;
						if( distance < lowestDistance )
						{
							lowestDistance = distance;
							nextCoord = testCoord;
						}						
					}
				}
			}
		}
		
		return true;		
	}
	
	bool IsValidMoveCheckNeighbours( TileCoord fromCoord, TileCoord targetCoord, int distanceMoved )
	{
		// Set tentative distance to self.
		_boardPieceField[ fromCoord.x ][ fromCoord.y ].SetTentativeDistance( distanceMoved++ );
		
		// If from and target match, we've arrived.
		if( fromCoord.Equals( targetCoord ) )
		{
			return true;
		}
		
		// Validate move to neighbour.
		List<TileCoord> ValidMoves = new List<TileCoord>();
		for( int i = 0; i < 4; ++i )	
		{
			if( IsValidMoveSingleStep( fromCoord, i ) )
			{
				ValidMoves.Add( fromCoord.Next( i ) );
			}
		}
		
		// Check neighbours of valid moves.
		foreach( var validMove in ValidMoves )
		{
			if( IsValidMoveCheckNeighbours( validMove, targetCoord, distanceMoved ) )
			{
				return true;
			}
		}
		
		return false;
	}
	
	bool IsValidMoveSingleStep( TileCoord fromCoord, int direction )
	{
		// Check that we can move from our tiles edge to the next tiles edge.
		// Condition: Not visited.
		// Condition: Both have valid transforms at matching edges.
		
		int[] targetEdgeIndices = new int[]
		{
			1, 0, 3, 2
		};
		
		var toCoord = fromCoord.Next( direction );
		
		if( _boardPieceField[ toCoord.x ][ toCoord.y ].HasVisited == true ||
			toCoord.x < 0 || toCoord.x >= Size ||
			toCoord.y < 0 || toCoord.y >= Size )
		{
			return false;
		}
		
		var fromEdgeTransform = _boardPieceField[fromCoord.x][fromCoord.y].GetEdgePieceTransform(direction);
		var toEdgeTransform = _boardPieceField[toCoord.x][toCoord.y].GetEdgePieceTransform(direction);
		
		if( fromEdgeTransform == null ||
			toEdgeTransform == null )
		{
			return false;
		}
		
		return true;
	}
	
	void OnGUI()
	{
		var rect = new Rect( 32.0f, 32.0f, 256.0f, 32.0f );
		GUI.TextField( rect, string.Format ( "Player {0}'s turn.", _activePlayerIndex + 1 ) );
	}
	
	
}
