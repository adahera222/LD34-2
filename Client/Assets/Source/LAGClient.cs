using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Net;

public class LAGClient : MonoBehaviour {

	public string ServerSource = "http://www.leadacidgames.com/game/ld28/serverlist.txt";
	public string DefaultServer = "http://localhost:8000";

	private string _server;

	public class GameInfo
	{
		public string Key;
		public string Name;
	}

	public class GameInstance
	{
		public bool InProgress;
		public string[] Players;
		public string[] Moves;
	}

	public delegate void QueryAllGamesResults( GameInfo[] gameInfo, string error );
	public delegate void CreateGameResults( string gameKey, string error );
	public delegate void QueryGameResults( GameInstance gameInstance, string error );


	// Use this for initialization
	void Start ()
	{
		_server = DefaultServer;
		
		CreateGame( "Test Game!", "password", testCreateGame );
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void testQueryAllGames( GameInfo[] gameInfo, string error )
	{
		int a=  0; ++a;
	}

	void testCreateGame( string gameKey, string error )
	{
		QueryAllGames( testQueryAllGames );
	}

	void QueryAllGames(QueryAllGamesResults queryAllGamesResults)
	{
		StartCoroutine(QueryAllGamesCoroutine(queryAllGamesResults));
	}
	
	void CreateGame(string name, string password, CreateGameResults createGameResults)
	{
		StartCoroutine(CreateGameCoroutine(name, password, createGameResults));
	}
	
	void JoinGame(string gameKey, QueryGameResults queryGameResults)
	{
		StartCoroutine(JoinGameCoroutine(gameKey, queryGameResults));	
	}

	void StartGame(string gameKey, QueryGameResults queryGameResults)
	{
		StartCoroutine(StartGameCoroutine(gameKey, queryGameResults));	
	}

	#region Coroutines


	public IEnumerator QueryAllGamesCoroutine(QueryAllGamesResults queryAllGamesResults)
	{
		HTTP.Request request = new HTTP.Request( "get", _server + "/queryAllGames" );
		request.Send();
		
		while( !request.isDone )
		{
			yield return null;
		}

		if( request.exception != null )
		{
			queryAllGamesResults( null, request.exception.ToString () );
		}

		var responseText = request.response.Text;
		var endOfCommand = responseText.IndexOf (":");
		var command = responseText.Substring (0, endOfCommand).Trim();
		var result = responseText.Substring( endOfCommand + 1 ).Trim();
		if (command == "/queryAllGames") 
		{
			var splitResults = result.Split(new string[]{ ",", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
			Debug.Log ( splitResults );
			List<GameInfo> results = new List<GameInfo>();
			for( int idx = 0; idx < splitResults.Length; idx += 2 )
			{
				var gameInfo = new GameInfo();
				gameInfo.Key = splitResults[ idx ];
				gameInfo.Name = splitResults[ idx + 1 ];
				results.Add( gameInfo );
				queryAllGamesResults( results.ToArray(), null );
			}
		}
		else 
		{
			queryAllGamesResults( null, "Error: " + result );
		}
	}

	public IEnumerator CreateGameCoroutine(string name, string password, CreateGameResults startGameResults)
	{
		HTTP.Request request = new HTTP.Request( "get", _server + string.Format ( "/createGame?name={0}&password={1}", name, password ) );
		request.Send();
		
		while( !request.isDone )
		{
			yield return null;
		}
		
		if( request.exception != null )
		{
			startGameResults( null, request.exception.ToString () );
		}
	
		var responseText = request.response.Text;
		var endOfCommand = responseText.IndexOf (":");
		var command = responseText.Substring (0, endOfCommand).Trim();
		var result = responseText.Substring( endOfCommand + 1 ).Trim();
		if (command == "/createGame") 
		{
			startGameResults( null, null );
		}
		else 
		{
			startGameResults( null, "Error: " + result );
		}
	}
	
	public IEnumerator JoinGameCoroutine(string gameKey, QueryGameResults queryGameResults)
	{	
		HTTP.Request request = new HTTP.Request( "get", _server + "/joinGame" );
		request.Send();
		
		while( !request.isDone )
		{
			yield return null;
		}
		
		if( request.exception != null )
		{
			queryGameResults( null, request.exception.ToString () );
		}
	
		var responseText = request.response.Text;
		var endOfCommand = responseText.IndexOf (":");
		var command = responseText.Substring (0, endOfCommand).Trim();
		var result = responseText.Substring( endOfCommand + 1 ).Trim();
		if (command == "/joinGame") 
		{
			queryGameResults( null, null );
		}
		else 
		{
			queryGameResults( null, "Error: " + result );
		}	
	}
	
	public IEnumerator StartGameCoroutine(string gameKey, QueryGameResults queryGameResults)
	{	
		HTTP.Request request = new HTTP.Request( "get", _server + "/createGame" );
		request.Send();
		
		while( !request.isDone )
		{
			yield return null;
		}
		
		if( request.exception != null )
		{
			queryGameResults( null, request.exception.ToString () );
		}
	
		var responseText = request.response.Text;
		var endOfCommand = responseText.IndexOf (":");
		var command = responseText.Substring (0, endOfCommand).Trim();
		var result = responseText.Substring( endOfCommand + 1 ).Trim();
		if (command == "/startGame") 
		{
			queryGameResults( null, null );
		}
		else 
		{
			queryGameResults( null, "Error: " + result );
		}	
	}

	#endregion
	
}
