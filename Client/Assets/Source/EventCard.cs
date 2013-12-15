using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventCard : MonoBehaviour 
{
	public List<EventPiece> EventPieces = new List<EventPiece>();
	
	public List<TextMesh> TextBits = new List<TextMesh>();
	
	private List<string> _eventStrings = new List<string>()
	{
		"Building of the Great Pyramid.",
		"Work on the Great Wall of China\nbegins.",
		"Roger Bacon invents gunpowder.",
		"English face Black Death Plague.",
		"Death of Napolean.",
		"Slavery absolished in USA.",
		"South Africa emerged from aparted\nregime with NelsonMandela\nas its president.",
		"Death of Sir Winston Churchill.",
		"First artificial satellite\nlaunched by Russian.",
		"World War II begins.",
		"Hitler becomes the Chanceller of\nGermany.",
		"Beginning of World War I.",
		"Death of Nelson Mandela.",
		"Euro becomes the official currency\nof 12 European countries.",
		"Cook's discovery of New South Wales,\nAustralia.",
		"Flinders circumnavigates Australia.",
		"Australian Prime Minister\nHarold Holt Disappears.",
		"English Civil War: The Battle\nof Alton takes place in Hampshire.",
		"Beatles World Tour begins in\nCopenhagen, Denmark.",
		"Oldest Chinese recording of a solar\neclipse.",
		"US Female Figure Skating\nchampionship won by Peggy Fleming.",
		"Buddy Holly records \"Blue Days\nBlack Night\" in Nashville.",
		"1st English Parliament called\ninto session by Earl of Leicester.",
		"1st music store in America opens.",
		"\"Star Trek II: The Wrath of Khan\"\nreleased in the USA.",
		"Boston Red Sox beat Brooklyn Dodgers\n4 games to 1 in 13th\nWorld Series.",
		"Belgium disbands salt tax.",
		"Interplanetary Hunger Commission\nmeets for the first time to\nfight universal poverty.",
		"Time travel invented. A new era,\nATT, begins.",
		"Largest known Unobtanium mining\noperation begins on Xenon-12.",
		"Ludum Dare community is now the\nlargest Earth colony.",
		"Venus and Mars sign treaty over\nEarth ownership.",
		"Pluto is officially redeclared a\nplanet.",
		"First known casualty of\ninterplanetary patent war.",
		"Reality television is banned on\n27/53 Human controlled planets.",
		"One year since first to-be time\ntravellers teamed to safeguard\nour future."
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
			var eventPiece = EventPieces[2 - i];
			eventPiece.SetTexture( eventTextures[events[i]], events[i] );		
			
			TextBits[2 - i].text = _eventStrings[ events[i] ];
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
