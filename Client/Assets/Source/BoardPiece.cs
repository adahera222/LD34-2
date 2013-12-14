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
		Right = 0x8
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
	
	
	private Connectivity[] _connectivity;
	
	
	// Use this for initialization
	void Start ()
	{
		// Grab public stuff, put in indexable container.
		_connectivity = new Connectivity[]
		{
			TopEdge,
			BottomEdge,
			LeftEdge,
			RightEdge
		};
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
