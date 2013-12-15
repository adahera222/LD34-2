using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventCard : MonoBehaviour 
{
	public List<EventPiece> EventPieces = new List<EventPiece>();
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}	
	
	public void SetupEvents( int[] events, List<Texture2D> eventTextures)
	{
		for( int i = 0; i < events.Length; ++i )
		{
			var eventPiece = EventPieces[i];
			eventPiece.SetTexture( eventTextures[events[i]], events[i] );		
		}
	}
	
	public bool CheckEvent( int eventId )
	{
		foreach( var eventPiece in EventPieces )	
		{
			if( eventPiece.EventId == eventId )
			{
				return true;
			}
		}
		
		return false;
	}
}
