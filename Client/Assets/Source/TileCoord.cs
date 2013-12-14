using System;

public struct TileCoord
{
	public int x;
	public int y;
	
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
	
	public TileCoord(int inX, int inY)
	{
		x = inX;
		y = inY;
	}
	
	public TileCoord Next(int direction)
	{
		return new TileCoord(x + Directions[direction].x, y + Directions[direction].y);
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
               objectToCompareWith.y == y;

    }
	
    public override int GetHashCode()
    {
        return x.GetHashCode() + y.GetHashCode();
    }
}

