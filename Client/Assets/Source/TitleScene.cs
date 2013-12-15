using UnityEngine;
using System.Collections;

public class TitleScene : MonoBehaviour 
{
	private string Instructions = 
		"Welcome Time Traveller!\n\n" +
		"For thousands of years people have be jumping in and out of the time stream, winning the \n" +
		"lottery, changing history and creating paradoxes, but the time steam can’t be diverted much \n" +
		"further. If something isn’t done now the whole thing will collapse into a big puddle of wibbly-\n" +
		"wobbly, timey-wimey at the bottom of a worm hole!\n\n" +
		"That’s where you come in! \n\n" +
		"You only get one chance to put things right. In order to win, you need to fix 10 points worth \n" +
		"of important historical events before the wormhole divertor known as the tile pile runs out, \n" +
		"but watch out, other time travelers will try to steal your glory!\n\n" +
		"Laid out before you is all of time and space, but you can’t access it all the time or from the \n" +
		"same point. On the table should be a map of what is currently available. Important events, \n" +
		"marked by dates are scattered across the map. Sadly the time stream is broken in many places or \n" +
		"branched incorrectly. Only connected time streams can be traveled through. \n" +
		"On the left there is a card listing three events in time, each allocated a number of points \n" +
		"that can be won by repairing the event. Once any one of these events has been repaired the goal \n" +
		"card will go to the bottom of the deck and new goals revealed, so get there fast. \n\n" +
		"There will also be a face up tile, this is the next available piece of time stream. On your \n" +
		"turn you must use this tile to change the map. Click on one of the arrows surrounding the map \n" +
		"in order to slide the tiles in that row or column in the direction indicated. The end tile will \n" +
		"return to the bottom of the tile deck and your shiny new one will slide in next to the arrow \n" +
		"you chose. \n\n" +
		"You can now move anywhere on the connected time stream. To repair an event simply land on it \n" +
		"during your turn. \n";			
			
			
	public TextMesh InstructionsMesh;
	
	// Use this for initialization
	void Start () 
	{
		InstructionsMesh.text = Instructions;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		var width = Camera.main.pixelWidth;
		var height = Camera.main.pixelHeight;
		
		var startGameWidth = 128.0f;	
		var startGameHeight = 32.0f;
		var startGameRect = new Rect( ( width / 2 ) - ( startGameWidth / 2 ), ( 7 * height / 8 ) - ( startGameHeight / 2 ), startGameWidth, startGameHeight ); 
				
		if( GUI.Button( startGameRect, "Start Game!" ) )
		{
			Application.LoadLevel( "GameScene" );
		}
	}
}
