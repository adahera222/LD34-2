using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventCard : MonoBehaviour 
{
	public List<EventPiece> EventPieces = new List<EventPiece>();
	
	public List<string> EventStrings = new List<string>()
	{
		"Building of the Great Pyramid.",
		"Work on the Great Wall of China begins.",
		"Roger Bacon invents gunpowder.",
		"English face Black Death Plague.",
		"Death of Napolean.",
		"Slavery absolished in USA.",
		"South Africa emerged from aparted regime with Nelson Mandela as its president.",
		"Death of Sir Winston Churchill.",
		"First artificial satellite launched by Russian.",
		"World War II begins.",
		"Hitler becomes the Chanceller or Germany.",
		"Beginning of World War I.",
		"Death of Nelson Mandela.",
		"Euro becomes the official currency of 12 European countries.",
		"Cook' discovery of New South Wales, Australia.",
		"Flinders circumnavigates Australia.",
		"Australian Prime Minister Harold Holt Disappears.",
		"English Civil War: The Battle of Alton takes place in Hampshire.",
		"Beatles World Tour begins in Copenhagen, Denmark.",
		"7810BC....date 20.",
		"US Female Figure Skating championship won by Peggy Fleming.",
		"Buddy Holly records \"Blue Days Black Night\" in Nashville.",
		"1st English Parliament called into session by Earl of Leicester.",
		"1st muse store in America opens.",
		"\"Star Trek II: The Wrath of Khan\" released in the USA.",
		"Boston Red Sox beat Brooklyn Dodgers 4 games to 1 in 13th World Series.",
		"Belgium disbands salt tax.",
		"Interplanetary Hunger Commission meets for the first time to fight universal poverty.",
		"Time travel invented. A new era, ATT, begins.",
		"Largest known Unobtanium mining operation begins on Xenon-12.",
		"Ludum Dare community is now the largest Earth colony.",
		"Venus and Mars sign treaty over Earth ownership.",
		"Pluto is officially redeclared a planet.",
		"First known casualty of interplanetary patent war.",
		"Reality television is banned on 27/53 Human controlled planets.",
		"One year since first to-be time travellers teamed to safeguard our future."
	};
	
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
