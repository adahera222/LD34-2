using UnityEngine;
using System.Collections;

public class TitleScene : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		var width = Camera.main.pixelWidth;
		var height = Camera.main.pixelHeight;
		
		var startGameWidth = 256.0f;	
		var startGameHeight = 64.0f;
		var startGameRect = new Rect( ( width / 2 ) - ( startGameWidth / 2 ), ( 3 * height / 4 ) - ( startGameHeight / 2 ), startGameWidth, startGameHeight ); 
				
		if( GUI.Button( startGameRect, "Start Game!" ) )
		{
			Application.LoadLevel( "GameScene" );
		}
	}
}
