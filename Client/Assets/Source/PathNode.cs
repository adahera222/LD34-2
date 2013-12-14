using System;
using System.Collections.Generic;

public class PathNode
{
	public TileCoord TileCoord = new TileCoord( -1, -1 );
	public bool HasVisited = false;
	public int TentativeDistance = int.MaxValue;
	
	public PathNode ()
	{
		
	}
	
	public void Reset()
	{
		HasVisited = false;	
		TentativeDistance = int.MaxValue;
	}
	
	public void SetTentativeDistance( int tentativeDistance )
	{
		HasVisited = true;
		if( tentativeDistance < TentativeDistance )
		{
			TentativeDistance = tentativeDistance;
		}
	}
}

