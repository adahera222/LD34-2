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
		Invalid = 0x10
	}
	
	private int _id = 0;
	
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
	
	
	private Connectivity GetConnectivity(int index)
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
		
		return Connectivity.Invalid;
	}
	
	private Transform GetEdgePieceTransform(int index)
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
	
	void OnDrawGizmos()
	{
		float radius = 0.05f;
		for( int i = 0; i < 4; ++i )
		{
			var edgeConnectivity = GetConnectivity(i);
			var edgePieceTransform = GetEdgePieceTransform(i);
			Vector3 a = transform.localToWorldMatrix.MultiplyPoint( _edgeVectors[ i ] * 0.5f );
			
			Gizmos.color = new Color( 0.0f, 1.0f, 0.0f, 1.0f );
			
			if( edgePieceTransform != null )
			{
				Gizmos.DrawSphere( edgePieceTransform.position, radius );
				Gizmos.DrawLine(a + new Vector3( 0.0f, radius, 0.0f ), edgePieceTransform.position + new Vector3( 0.0f, radius, 0.0f ));
			}
			
			Gizmos.color = new Color( 0.0f, 0.0f, 1.0f, 1.0f );
			for( int j = 0; j < 4; ++j )
			{
				Vector3 b = transform.localToWorldMatrix.MultiplyPoint( _edgeVectors[ j ] * 0.5f );
				Gizmos.DrawSphere( a, radius );
				if( ( edgeConnectivity & (Connectivity)(1 << j) ) != 0 )
				{
					Gizmos.DrawLine(a + new Vector3( 0.0f, radius, 0.0f ), b + new Vector3( 0.0f, radius, 0.0f ));
				}
			}			
		}
	}
}
