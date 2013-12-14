using System;

public class TileCoord
{
	public int x;
	public int y;
	public int edge;
		
	public enum Direction
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3
	}
	
	public static readonly TileCoord[] Directions = new TileCoord[]
	{
		new TileCoord( 0, 1 ),	
		new TileCoord( 0, -1 ),
		new TileCoord( -1, 0 ),	
		new TileCoord( 1, 0 ),
	};
	
	public static readonly int[] OppositeEdge = new int[]
	{
		1, 0, 3, 2
	};
	
	public TileCoord(int inX, int inY)
	{
		x = inX;
		y = inY;
		edge = -1;
	}
	
	public TileCoord(int inX, int inY, int inEdge)
	{
		x = inX;
		y = inY;
		edge = inEdge;
	}
		
	public TileCoord Next(int direction)
	{
		return new TileCoord(x + Directions[direction].x, y + Directions[direction].y, OppositeEdge[ edge ]);
	}
	
 	public bool Equals(TileCoord other)
    {
        return Equals(other, this);
    }

	public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var objectToCompareWith = (TileCoord)obj;

        return objectToCompareWith.x == x &&
               objectToCompareWith.y == y &&
		       objectToCompareWith.edge == edge;

    }
	
    public override int GetHashCode()
    {
        return x.GetHashCode() + y.GetHashCode() + edge.GetHashCode();
    }
}

