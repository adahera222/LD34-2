using UnityEngine;
using System.Collections;

public class BoardPiece : MonoBehaviour
{
	[System.Flags]
	public enum Connectivity
	{
		Top = 0x1,
		Bottom = 0x2,
		Left = 0x4,
		Right = 0x8,
	}
	
	private int _id = 0;
	
	
	public int EventId = -1;	
	
	public bool CanHaveEvent = false;
	
	public TileCoord Coord;
	
	[EnumFlags]
	public Connectivity TopEdge;

	[EnumFlags]
	public Connectivity BottomEdge;
	
	[EnumFlags]
	public Connectivity LeftEdge;
	
	[EnumFlags]
	public Connectivity RightEdge;
	
	public Transform TopEdgePieceTransform;
	public Transform BottomEdgePieceTransform;
	public Transform LeftEdgePieceTransform;
	public Transform RightEdgePiecePTransform;
	
	public bool IsStart = false;
	public int TotalInDeck = 0;
	public int TotalWithEventsInDeck = 0;	
	
	
	#region Path finding.
	public PathNode[] PathNodes = new PathNode[4];
		
	public void ClearPathfinding()
	{
		for( int i = 0; i < 4; ++i )
		{
			PathNodes[i] = new PathNode();
		}
	}
	
	#endregion
	
	public Connectivity GetConnectivity(int index)
	{
		switch( index )
		{
		case 0:
			return TopEdge;
		case 1:
			return BottomEdge;
		case 2:
			return LeftEdge;
		case 3:
			return RightEdge;
		}
		
		return 0;
	}
	
	public bool IsConnected(int fromEdge, int toEdge )
	{
		return ( fromEdge == toEdge ) || ( (int)GetConnectivity( fromEdge ) & (1 << toEdge) ) != 0 ? true : false;
	}
	
	public Transform GetEdgePieceTransform(int index)
	{
		switch( index )
		{
		case 0:
			return TopEdgePieceTransform;
		case 1:
			return BottomEdgePieceTransform;
		case 2:
			return LeftEdgePieceTransform;
		case 3:
			return RightEdgePiecePTransform;
		}
		
		return null;
	}

	private BoardPlayArea _playArea;
	
	private readonly Vector3[] _edgeVectors = new Vector3[4]
	{
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, -1.0f),
		new Vector3(-1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
	};
	
	
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
			_playArea = playArea;
		}
		
		// ID.
		if( id != -1 )
		{
			_id = id;
		}
			
		// Setup name for easy debugging.
		this.gameObject.name = string.Format( "BoardPiece({0})", _id );
	}
	
	public void SetupEvent( int id, EventPiece templateEventObject, Texture2D eventTexture )
	{
		var gameObject = Object.Instantiate( templateEventObject.gameObject ) as GameObject;
		var eventPiece = gameObject.GetComponentInChildren< EventPiece >();
		eventPiece.SetTexture( eventTexture );		
		
		eventPiece.transform.parent = this.transform;
		eventPiece.transform.position = this.transform.position;
		eventPiece.transform.rotation = Quaternion.Euler( new Vector3( 180.0f, 0.0f, 0.0f ) );
		
	}
	
	void OnDrawGizmos()
	{
		float radius = 0.05f;
		for( int i = 0; i < 4; ++i )
		{
			var edgeConnectivity = GetConnectivity(i);
			var edgePieceTransform = GetEdgePieceTransform(i);
			Vector3 a = transform.localToWorldMatrix.MultiplyPoint( _edgeVectors[ i ] * 0.4f );
			
			if( PathNodes[ i ] != null )
			{
				if( PathNodes[ i ].HasVisited == false )
				{
					Gizmos.color = new Color( 0.0f, 0.0f, 0.0f, 1.0f );
				}
				else
				{
					Gizmos.color = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
				}
				Gizmos.DrawSphere( a, 0.1f );
			}
			
			
			Gizmos.color = new Color( 0.0f, 1.0f, 0.0f, 1.0f );
			
			if( edgePieceTransform != null )
			{
				Gizmos.DrawSphere( edgePieceTransform.position, radius );
				Gizmos.DrawLine(a + new Vector3( 0.0f, radius, 0.0f ), edgePieceTransform.position + new Vector3( 0.0f, radius, 0.0f ));
			}
			
			Gizmos.color = new Color( 0.0f, 0.0f, 1.0f, 1.0f );
			for( int j = 0; j < 4; ++j )
			{
				Vector3 b = transform.localToWorldMatrix.MultiplyPoint( _edgeVectors[ j ] * 0.4f );
				Gizmos.DrawSphere( a, radius );
				if( ( edgeConnectivity & (Connectivity)(1 << j) ) != 0 )
				{
					Gizmos.DrawLine(a + new Vector3( 0.0f, radius, 0.0f ), b + new Vector3( 0.0f, radius, 0.0f ));
				}
			}			
		}
	}
}
