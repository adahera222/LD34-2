using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoardPlayArea : MonoBehaviour 
{
	public BoardPiecePlayLocation TemplatePiecePlayLocation;
	public List<BoardPiece> TemplateBoardPieces;
	public List<PlayerPiece> TemplatePlayerPieces;
	public PlayerScoreBoard TemplatePlayerScoreBoard;
	public EventCard TemplateEventCard;
	
	public Transform EventCardScreenPosition;
	public Transform EventCardPilePosition;
	public Transform EventCardRevealPosition;
	public Transform TilePilePosition;
	public Transform TileRevealedPosition;
	
	public List<Transform> PlayerScorePositions = new List<Transform>();
	
	public EventPiece TemplateEventObject;
	
	public List<Texture2D> EventTextures = new List<Texture2D>();
	
	public enum PlayAreaState
	{
		BeginTurn,
		ShowNewEventCard,
		WaitNewEventCard,
		PlaceNewTile,
		MovePlayer,
		NextTurn,
		PlayerWon
	}
	
	public int Size = 7;	
	
	public float MoveHeightForTileSlide = 0.0f;
	public float MoveHeightForNewTile = 1.0f;
	public float MoveHeightForReturnTile = 2.0f;
	
	private PlayAreaState _playAreaState = PlayAreaState.BeginTurn;
	
	private float _timer = 0.0f;
	
	// Pieces in pile to select from.
	private List<BoardPiece> _boardPiecePile = new List<BoardPiece>();
	
	private List<BoardPiecePlayLocation> _boardPiecePlayLocations = new List<BoardPiecePlayLocation>();
	
	// Played pieces.
	private BoardPiece[][] _boardPieceField = null;
	
	// Player pieces.
	private List<PlayerPiece> _playerPieces = new List<PlayerPiece>();
	private List<PlayerScoreBoard> _playerScoreBoard = new List<PlayerScoreBoard>();
	
	// Event cards.
	private List<EventCard> _eventCards = new List<EventCard>();
	
	private EventCard _eventCardActive = null;
		
	// Active player index.
	private int _activePlayerIndex = 0;
	
	// event card.
	private bool _needNewEventCard = true;
	
	// Use this for initialization
	void Start ()
	{		
		Random.seed = 0; // hard code seed for now.
		int id = 0;
		
		// Create board pieces.
		int eventId = 0;
		foreach( var templateBoardPiece in TemplateBoardPieces)
		{
			for( int i = 0; i < templateBoardPiece.TotalInDeck; ++i )
			{
				var boardPieceObject = Object.Instantiate( templateBoardPiece.gameObject ) as GameObject;
				var boardPiece = boardPieceObject.GetComponent< BoardPiece >();
				_boardPiecePile.Add( boardPiece );
				boardPiece.SetupPiece( this, id++ );
				
				if( i < boardPiece.TotalWithEventsInDeck )
				{
					boardPiece.SetupEvent( eventId, TemplateEventObject, EventTextures[ eventId ] );
					++eventId;
				}
			
				var position = GetPilePosition();
				boardPiece.transform.parent = transform;
				boardPiece.transform.localPosition = position;			
				boardPiece.transform.rotation = TilePilePosition.rotation;
			}
		}
		
		// Shuffle pieces.
		ShufflePieces();
		
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
		
		// Create cards.
		HashSet<int> usedEvents = new HashSet<int>();
		for( int i = 0; i < 12; ++i )
		{
			int[] events = new int[3]
			{
				-1, -1, -1
			};
			
			for( int j = 0; j < 3; ++j )
			{
				for( int k = 0; k < _boardPiecePile.Count; ++k )		
				{
					var boardPiece = _boardPiecePile[ k ];
					if( boardPiece.EventPiece != null )
					{
						if( boardPiece.ScoreValue == j + 1 )
						{
							if( usedEvents.Contains( boardPiece.EventPiece.EventId ) == false )
							{
								events[j] = boardPiece.EventPiece.EventId;
								usedEvents.Add ( boardPiece.EventPiece.EventId );
								break;
							}	
						}
					}
				}
			}
			
			var eventCardObject = Object.Instantiate( TemplateEventCard.gameObject ) as GameObject;
			var eventCard = eventCardObject.GetComponent< EventCard >();
			eventCard.SetupEvents( events, EventTextures );
			_eventCards.Add ( eventCard );
			
			eventCard.transform.parent = this.transform;
			eventCard.transform.position = EventCardPilePosition.position;
			eventCard.transform.rotation = EventCardPilePosition.rotation;
		}
				
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
					
					if( x == -1 )
					{
						boardPiece.transform.rotation = Quaternion.Euler( new Vector3( 0.0f, 270.0f, 0.0f ) );
					}
					else if( x == Size )
					{
						boardPiece.transform.rotation = Quaternion.Euler( new Vector3( 0.0f, 90.0f, 0.0f ) );
					}
					else if( y == -1 )
					{
						boardPiece.transform.rotation = Quaternion.Euler( new Vector3( 0.0f, 180.0f, 0.0f ) );
					}
					else if( y == Size )
					{
						boardPiece.transform.rotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, 0.0f ) );
					}
					
					_boardPiecePlayLocations.Add(boardPiece);
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
			objectMover.Move(TileRevealedPosition.position, TileRevealedPosition.rotation, MoveHeightForNewTile, null);
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
				playerPiece.CurrCoord = new TileCoord( centre, centre, i );
				
				playerPiece.isAI = i != 0;
								
				_playerPieces.Add ( playerPiece );
				
				var playerScoreObject = Object.Instantiate( TemplatePlayerScoreBoard.gameObject ) as GameObject;
				var playerScore = playerScoreObject.GetComponent< PlayerScoreBoard >();
				playerScore.PlayerPiece = playerPiece;
				
				playerScore.transform.parent = PlayerScorePositions[i].transform;
				playerScore.transform.localPosition = Vector3.zero;
				playerScore.transform.localRotation = Quaternion.identity;
				
				_playerScoreBoard.Add ( playerScore );
			}
		}
		
		_playerScoreBoard[0].SetActive();
	}
	
	void ShufflePieces()
	{
		for( int i = 0; i < _boardPiecePile.Count * 4; ++i )
		{
			var randomIdx = Random.Range( 0, _boardPiecePile.Count);
			_boardPiecePile.Insert( randomIdx, _boardPiecePile[0] );
			_boardPiecePile.RemoveAt(0);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		var playerPiece = _playerPieces[ _activePlayerIndex ];

		_timer -= Time.deltaTime;
		
		switch( _playAreaState )
		{
		case PlayAreaState.BeginTurn:
		{
			// Activate.
			_playerScoreBoard[ _activePlayerIndex ].SetActive();
			
			_playAreaState = PlayAreaState.PlaceNewTile;
			
			ClearPathfinding();
			SetupPathGlow();
			
			for(int i = 0; i < _boardPiecePlayLocations.Count; ++i)
			{
				var glower = _boardPiecePlayLocations[i].GetComponentInChildren<Glower> ();
				glower.GlowTarget = 2.0f;
			}
			
			if( _needNewEventCard )
			{
				_playAreaState = PlayAreaState.ShowNewEventCard;
			}
			
			if( playerPiece.isAI )
			{
				_timer = 2.0f;
			}
		}
		break;
			
		case PlayAreaState.ShowNewEventCard:
		{
			_playAreaState = PlayAreaState.WaitNewEventCard;
			_needNewEventCard = false;
			RevealEventCard();
		}
		break;
			
		case PlayAreaState.WaitNewEventCard:
		{
			if(_timer < 0.0f )
			{
				if( _eventCardActive )
				{
					var objectMover = _eventCardActive.GetComponent< ObjectMover >();
					objectMover.Move( EventCardRevealPosition.position, EventCardRevealPosition.rotation, MoveHeightForReturnTile, null );
					_playAreaState = PlayAreaState.PlaceNewTile;
				}
			}
		}
		break;
		
		case PlayAreaState.PlaceNewTile:
		{
			BoardPiecePlayLocation boardPiece = null;
			
			if( playerPiece.isAI == false )
			{
				/////////////////////////////////////////////// HUMAN
				if( Input.GetMouseButtonDown( 0 ) )
				{
					var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
					var rayHits = Physics.RaycastAll( ray, Mathf.Infinity, 1 << Layers.PiecePlayLocation );
					if( rayHits.Length > 0 )
					{
						var rayHit = rayHits[0];
						boardPiece = rayHit.collider.gameObject.GetComponent< BoardPiecePlayLocation >();
					}
				}
			}
			else
			{
				if( _timer < 0.0f )
				{
					/////////////////////////////////////////////// AI
					var randomIdx = Random.Range( 0, _boardPiecePlayLocations.Count );
					boardPiece = _boardPiecePlayLocations[randomIdx];
				
					_timer = 2.0f;
				}
			}
			
			// Do move.
			if( boardPiece != null )
			{
				PlayPiece( boardPiece.X, boardPiece.Y, null );
				
				// Check that we can make a move.
				bool canMove = false;
				var fromCoord = _playerPieces[ _activePlayerIndex ].CurrCoord;
				for( int i = 0; i < 4; ++i )
				{
					if( IsValidMoveSingleStep( fromCoord, i, false ) != null )	
					{
						canMove = true;
					}						
				}
			
				if(canMove)
				{
					_playAreaState = PlayAreaState.MovePlayer;
				
					// Touch all valid move bits.
					fromCoord = _playerPieces[ _activePlayerIndex ].CurrCoord;
					CalulatePath( fromCoord, null );
				
					SetupPathGlow();
				

				}
				else
				{
					_playAreaState = PlayAreaState.NextTurn;
				}
				
				for(int i = 0; i < _boardPiecePlayLocations.Count; ++i)
				{
					var glower = _boardPiecePlayLocations[i].GetComponentInChildren<Glower> ();
					glower.GlowTarget = 0.0f;
				}

			}
		}
		break;
		
		case PlayAreaState.MovePlayer:
		{
			if( playerPiece.isAI == false )
			{
				/////////////////////////////////////////////// HUMAN
				if( Input.GetMouseButtonDown( 0 ) )
				{
					var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
					var rayHits = Physics.RaycastAll( ray, Mathf.Infinity, 1 << Layers.MoveLocation );
					if( rayHits.Length > 0 )
					{
						var rayHit = rayHits[0];
					
						var fromBoardPiece = _playerPieces[ _activePlayerIndex ].transform.parent.GetComponent< BoardPiece >();
						var movePosition = rayHit.collider.gameObject.GetComponent< PieceMovePosition >();
						var targetBoardPiece = movePosition.transform.parent.GetComponent< BoardPiece >();
						if( targetBoardPiece != null )
						{
							var newCoord = targetBoardPiece.Coord;
					
							if( newCoord != null )
							{
								for( int i = 0; i < 4; ++i )
								{
									if( targetBoardPiece.GetEdgePieceTransform( i ) == movePosition.transform )
									{
										newCoord = new TileCoord( newCoord.x, newCoord.y, i );
										break;
									}
								}
								
								var path = CalulatePath( playerPiece.CurrCoord, newCoord );
								
								MoveActivePlayer(path);
							}
						}
					}								
				}
			}
			else
			{
				if( _timer < 0.0f )
				{
					/////////////////////////////////////////////// AI!
				
					// Path outwards from the player to mark cells.
					var path = CalulatePath( playerPiece.CurrCoord, null );
					
					// Build list of movable board pieces.
					List<BoardPiece> movableBoardPieces = new List<BoardPiece>();
					for( int y = 0; y < Size; ++y )
					{
						for( int x = 0; x < Size; ++x )
						{
							var boardPiece = _boardPieceField[x][y];
							
							if( boardPiece.HasVisited() )
							{
								boardPiece.AIScoreValue = Random.Range( 0, 8 );
	
								// Event piece? Bump that score up.
								if( boardPiece.EventPiece != null )
								{
									for( int i = 0; i < 3; ++i )
									{
										var eventScore = ( 3 - i ) * 10;
										var eventPiece = _eventCardActive.EventPieces[i];
										
										if( eventPiece.EventId == boardPiece.EventPiece.EventId )
										{
											boardPiece.AIScoreValue += eventScore;
										}
									}
								}
								
								movableBoardPieces.Add (boardPiece);
							}
						}
					}
					
					// Sort.
					movableBoardPieces = movableBoardPieces.OrderBy( x => -x.AIScoreValue ).ToList ();
					
					// Target coord.
					var targetPiece = movableBoardPieces[0];
					TileCoord targetCoord = targetPiece.Coord;
					for( int i = 0; i < 4; ++i )
					{
						if( targetPiece.PathNodes[i].HasVisited )
						{
							targetCoord = new TileCoord( targetCoord.x, targetCoord.y, i );
							break;
						}
					}
					
					// Path
					path = CalulatePath( playerPiece.CurrCoord, targetCoord );
					
					MoveActivePlayer(path);
				}
			}
		}
		break;

		case PlayAreaState.NextTurn:
		{
			ClearPathfinding();
			SetupPathGlow();

			_playAreaState = PlayAreaState.BeginTurn;
		
			//
			if( _playerPieces[ _activePlayerIndex ].Score >= 10 )
			{
				_playAreaState = PlayAreaState.PlayerWon;
				break;
			}
			
			// Deactivate.
			_playerScoreBoard[ _activePlayerIndex ].SetInactive();
			
			// Player.
			_activePlayerIndex = ( _activePlayerIndex + 1 ) % 4;
		}
		break;
		
		case PlayAreaState.PlayerWon:
		{
			
		}
		break;
		}
	}
	
	void MoveActivePlayer( List<TileCoord> path )
	{
		var playerPiece = _playerPieces[ _activePlayerIndex ];
		if( path != null )
		{
			var lastPos = path[ path.Count - 1 ];
		
			// Just jump to target coord.
			var transformOnTile = GetTransformOnTile( lastPos.x, lastPos.y, lastPos.edge );
			if( transformOnTile != null )
			{
				var boardPiece = _boardPieceField[lastPos.x][lastPos.y];
				playerPiece.transform.parent = boardPiece.transform;
				var objectMover = playerPiece.gameObject.GetComponent< ObjectMover >();
				objectMover.Move( transformOnTile.position, Quaternion.identity, 2.0f, null );
				playerPiece.CurrCoord = lastPos;
				_playAreaState = PlayAreaState.NextTurn;
			
				if( boardPiece.EventPiece != null )
				{
					if( _eventCardActive.CheckEvent( boardPiece.EventPiece.EventId ) )
					{
						// Increment score.
						_playerPieces[ _activePlayerIndex ].Score += boardPiece.ScoreValue;
						
						var particleSystem = _playerScoreBoard[ _activePlayerIndex ].GetComponentInChildren< ParticleSystem >();
						if( particleSystem != null )
						{
							particleSystem.Emit(25);
						}
					
						_needNewEventCard = true;
					}
				}

			}
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
		return TilePilePosition.position;
	}
	
	public void RevealEventCard()
	{
		var newEventCard = _eventCards[0];	
		_eventCards.RemoveAt(0);
		
		var objectMover = newEventCard.GetComponent< ObjectMover >();
		
		objectMover.Move( this.EventCardScreenPosition.position, EventCardScreenPosition.rotation, MoveHeightForNewTile, null );
		
		
		if( _eventCardActive )
		{
			objectMover = _eventCardActive.GetComponent< ObjectMover >();
			objectMover.Move( EventCardPilePosition.position, EventCardPilePosition.rotation, MoveHeightForReturnTile, null );
			_eventCards.Add(_eventCardActive);
		}
		
		_eventCardActive = newEventCard;
		
		_timer = 4.0f;
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
						objectMover.Move(position, TilePilePosition.rotation, MoveHeightForReturnTile, null);

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
						objectMover.Move(position, TilePilePosition.rotation, MoveHeightForReturnTile, null);

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
						objectMover.Move(position, TilePilePosition.rotation, MoveHeightForReturnTile, null);

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
						objectMover.Move(position, TilePilePosition.rotation, MoveHeightForReturnTile, null);

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
			objectMover.Move(TileRevealedPosition.position, TileRevealedPosition.rotation, MoveHeightForNewTile, null);
		}
		
		// Update player coords.
		int centre = Size / 2;
		for( int i = 0; i < _playerPieces.Count; ++i)
		{
			var playerPiece = _playerPieces[ i ];
			var boardPiece = playerPiece.transform.parent.GetComponent< BoardPiece >();
			playerPiece.CurrCoord = new TileCoord( boardPiece.Coord.x, boardPiece.Coord.y, playerPiece.CurrCoord.edge );
			
			if( playerPiece.CurrCoord.x == -1 ||
				playerPiece.CurrCoord.y == -1 ||
				playerPiece.CurrCoord.edge == -1 )
			{
				playerPiece.transform.parent = _boardPieceField[centre][centre].transform;
				var transformOnTile = _boardPieceField[centre][centre].GetEdgePieceTransform( i );
				var objectMover = playerPiece.gameObject.GetComponent< ObjectMover >();
				objectMover.Move( transformOnTile.position, Quaternion.identity, 2.0f, null );
				playerPiece.CurrCoord =  new TileCoord( centre , centre , i );
			}
		}
	}
	
	void SetupPathGlow()
	{
		for( int y = 0; y < Size; ++y )
		{
			for( int x = 0; x < Size; ++x )
			{
				bool shouldGlow = false;
				for( int i = 0; i < 4; ++i )
				{
					var edgePieceTransform = _boardPieceField[x][y].GetEdgePieceTransform(i);
					if( _boardPieceField[x][y].PathNodes[i].HasVisited )
					{
						shouldGlow = true;
						
						if(edgePieceTransform != null)
						{
							var particleSystem = edgePieceTransform.GetComponentInChildren<ParticleSystem>();
							particleSystem.enableEmission = true;
							particleSystem.Play();
						}
					}										
					else
					{
						if(edgePieceTransform != null)
						{
							var particleSystem = edgePieceTransform.GetComponentInChildren<ParticleSystem>();
							particleSystem.enableEmission = false;
							particleSystem.Stop();
						}
					}
				}
			
				if( shouldGlow )
				{
					_boardPieceField[x][y].Glower.GlowTarget = 1.0f;
				}
				else
				{
					_boardPieceField[x][y].Glower.GlowTarget = 0.0f;
				}
			}
		}
	}
	
	void ClearPathfinding()
	{
		// Clear path finding data.
		for( int y = 0; y < Size; ++y )
		{
			for( int x = 0; x < Size; ++x )
			{
				_boardPieceField[x][y].ClearPathfinding();
			}
		}
	}
	
	List<TileCoord> CalulatePath( TileCoord fromCoord, TileCoord targetCoord )
	{
		ClearPathfinding();
		
		// Check neighbours.
		TileCoord finalCoord = IsValidMoveCheckNeighbours( fromCoord, targetCoord, 0 );
		if( finalCoord != null )
		{
			// If we have found, work back from the target coord down the quickest path.
			List<TileCoord> path = new List<TileCoord>();
			TileCoord currCoord = finalCoord;
			TileCoord nextCoord = finalCoord;
			
			path.Add ( currCoord );
			
			while( !( fromCoord.x == currCoord.x &&
			          fromCoord.y == currCoord.y &&
			          fromCoord.edge == currCoord.edge ) )
			{
				int lowestDistance = _boardPieceField[ currCoord.x ][ currCoord.y ].PathNodes[ currCoord.edge ].TentativeDistance;
				
				for( int i = 0; i < 4; ++i )
				{
					var validMoveOut = IsValidMoveSingleStep( currCoord, i, false );
					
					if( validMoveOut != null )
					{
						var testCoord = validMoveOut.Next( i );
						int distance = _boardPieceField[ testCoord.x ][ testCoord.y ].PathNodes[ testCoord.edge ].TentativeDistance;
						if( distance < lowestDistance )
						{
							lowestDistance = distance;
							nextCoord = testCoord;
						}						
					}
				}

				// If we didn't move, break out.
				if( lowestDistance == _boardPieceField[ currCoord.x ][ currCoord.y ].PathNodes[ currCoord.edge ].TentativeDistance )
				{
					break;
				}
				
				path.Add ( nextCoord );				
				currCoord = nextCoord;
			}
			
			// Reverse so we can traverse it.
			path.Reverse();
			
			return path.Count > 0 ? path : null;
		}
		
		return null;		
	}
	
	TileCoord IsValidMoveCheckNeighbours( TileCoord fromCoord, TileCoord targetCoord, int distanceMoved )
	{
		// Set tentative distance to self.
		_boardPieceField[ fromCoord.x ][ fromCoord.y ].PathNodes[ fromCoord.edge ].SetTentativeDistance( distanceMoved );
		
		//Debug.Log ( string.Format( "Search Path: {0}, {1}, {2} : {3}", fromCoord.x, fromCoord.y, fromCoord.edge, distanceMoved ) );
		
		// If from and target match, we've arrived.
		if( targetCoord != null &&
			fromCoord.x == targetCoord.x &&
			fromCoord.y == targetCoord.y )
		{
			var fromTile = _boardPieceField[fromCoord.x][fromCoord.y];
			
			if( fromTile.IsConnected( fromCoord.edge, targetCoord.edge ) )
			{
				return targetCoord;
			}
		}
		
		// Validate move to neighbour.
		List<TileCoord> validMovesOut = new List<TileCoord>();
		for( int i = 0; i < 4; ++i )	
		{
			var outTileCoord = IsValidMoveSingleStep( fromCoord, i, true );
			
			if( outTileCoord != null )
			{
				validMovesOut.Add( outTileCoord );
			}
		}
		
		// Check neighbours of valid moves.
		foreach( var validMoveOut in validMovesOut )
		{
			_boardPieceField[ validMoveOut.x ][ validMoveOut.y ].PathNodes[ validMoveOut.edge ].SetTentativeDistance( distanceMoved );
			
			TileCoord retVal = IsValidMoveCheckNeighbours( validMoveOut.Next( validMoveOut.edge ), targetCoord, distanceMoved + 1 );
			if( retVal != null )
			{
				return retVal;
			}
		}
		
		return null;
	}
	
	TileCoord IsValidMoveSingleStep( TileCoord fromCoord, int direction, bool checkIfVisited )
	{
		// Check that we can move from our tiles edge to the next tiles edge.
		// Condition: Internally, we have the valid connectivity.
		// Condition: Not visited the next.
		// Condition: Both have valid transforms at matching edges.
		int[] targetEdgeIndices = new int[]
		{
			1, 0, 3, 2
		};
		
		var fromTile = _boardPieceField[fromCoord.x][fromCoord.y];
		
		var nextFromCoord = new TileCoord( fromCoord.x, fromCoord.y, direction );
		
		// Check connectivity, update from.
		if( fromTile.IsConnected( fromCoord.edge, nextFromCoord.edge ) == false )
		{
			return null;
		}
		else
		{
			fromCoord = nextFromCoord;
		}	
				
		// Generate to.		
		var toCoord = fromCoord.Next( direction );
		
		if( toCoord.x < 0 || toCoord.x >= Size ||
			toCoord.y < 0 || toCoord.y >= Size ||
			( checkIfVisited && _boardPieceField[ toCoord.x ][ toCoord.y ].PathNodes[ toCoord.edge ].HasVisited ) )
		{
			return null;
		}
		
		var fromEdgeTransform = fromTile.GetEdgePieceTransform(direction);
		var toEdgeTransform = _boardPieceField[toCoord.x][toCoord.y].GetEdgePieceTransform(toCoord.edge);
		
		if( fromEdgeTransform == null ||
			toEdgeTransform == null )
		{
			return null;
		}
		
		return fromCoord;
	}
	
	void OnGUI()
	{
		var width = Camera.main.pixelWidth;
		var height = Camera.main.pixelHeight;
		
		var startGameWidth = 128.0f;	
		var startGameHeight = 32.0f;
		var startGameRect = new Rect( width - startGameWidth, height  - startGameHeight, startGameWidth, startGameHeight ); 
				
		if( GUI.Button( startGameRect, "Quit Game!" ) )
		{
			Application.LoadLevel( "TitleScene" );
		}
	}
}
