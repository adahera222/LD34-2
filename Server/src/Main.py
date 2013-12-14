import BaseHTTPServer
import hashlib
import base64
import datetime
import string

SALT = "saltysaltsalt"		# note: this is something different on the server :)
TIMEOUT = datetime.timedelta( minutes = 30 )
GAME_INSTANCES = {}

def cleanGameInstances():
	global GAME_INSTANCES
	now = datetime.datetime.now()
	copiedGameInstances = GAME_INSTANCES.copy();
	for gameInstanceKey in copiedGameInstances.keys():
		gameInstance = GAME_INSTANCES[ gameInstanceKey ]
		difference = ( now - gameInstance.lastAccess )
		if difference > TIMEOUT:
			del GAME_INSTANCES[ gameInstanceKey ]

class GameInstance:
	def __init__( self, name, password ):
		self.name = name
		m = hashlib.sha256()
		m.update( password )
		m.update( SALT )
		self.password = m.digest()
		self.players = []
		self.moves = []
		self.inProgress = False
		self.lastAccess = datetime.datetime.now()

	def __str__( self ):
		return str( ( str( self.players ), str( self.moves ) ) )

	def validatePassword( self, command ):
		if len( self.password ) > 0:
			if command.params.has_key( "password" ) == False:
				return False
			m = hashlib.sha256()
			m.update( command.params[ "password" ] )
			m.update( SALT )
			if m.digest() != self.password:
				return False
		return True

	def onQuery( self, command ):
		if self.validatePassword( command ) == False:
			return "InvalidPassword"
		self.lastAccess = datetime.datetime.now()
		return str( self.inProgress ) + ":" + string.join( self.players ) + ":" + string.join( self.moves )

	def onStart( self, command ):
		if self.validatePassword( command ) == False:
			return "InvalidPassword"
		if self.inProgress == True:
			return "InProgress"
		self.inProgress = True
		return self.onQuery( command )

	def onJoin( self, command ):
		if self.validatePassword( command ) == False:
			return "InvalidPassword"
		if self.inProgress == True:
			return "InProgress"
		if command.params.has_key( "playerName" ) == False:
			return "NoPlayerName"
		playerName = command.params[ "playerName" ]
		if playerName in self.players:
			return "InGame"
		self.players.append( playerName )
		return self.onQuery( command )

	def onMove( self, command ):
		if self.validatePassword( command ) == False:
			return "InvalidPassword"
		if self.inProgress == False:
			return "NotInProgress"
		if command.params.has_key( "playerName" ) == False:
			return "NoPlayerName"
		if command.params.has_key( "move" ) == False:
			return "NoMove"
		playerName = command.params[ "playerName" ]
		if playerName in self.players:
			move = command.params[ "move" ]
			self.moves.append( move )
			return self.onQuery( command )
		else:
			return "NotInGame"

class HttpCommand:
	def __init__( self, inPath ):
		paramIdx = inPath.find( "?" )
		if paramIdx >= 0:
			self.cmd = inPath[:paramIdx]
			self.paramString = inPath[paramIdx+1:]
		else:
			self.cmd = inPath
			self.paramString = ""
		self.params = {}
		if len( self.paramString ) > 0:
			paramKeyValues = self.paramString.split( "&" )
			for paramKeyValue in paramKeyValues:
				param = paramKeyValue.split( "=" )
				self.params[ param[0] ] = param[1]

	def __str__( self ):
		return "Command: %s\nParam String: %s\nParams: %s\n" % ( self.cmd, self.paramString, self.params )

class HttpCommandHandler(BaseHTTPServer.BaseHTTPRequestHandler):
	def __init__( self, a, b, c ):
		self.commandHandlers = {
			"/queryAllGames" : self.onQueryAllGames,
			"/queryGame" : self.onQueryGame,
			"/createGame" : self.onCreateGame,
			"/startGame" : self.onStartGame,
			"/joinGame" : self.onJoinGame,
			"/moveGame" : self.onMoveGame
		}
		BaseHTTPServer.BaseHTTPRequestHandler.__init__( self, a, b, c )

	def onQueryAllGames( self, command ):
		cleanGameInstances()
		global GAME_INSTANCES
		games = []
		for gameInstanceKey in GAME_INSTANCES.keys():
			gameInstance = GAME_INSTANCES[ gameInstanceKey ]
			if gameInstance.inProgress == False:
				games.append( gameInstanceKey );
				games.append( gameInstance.name );
		return string.join( games, "," )

	def onQueryGame( self, command ):
		global GAME_INSTANCES
		if command.params.has_key( "gameKey" ):
			gameKey = command.params[ "gameKey" ]
			gameInstance = GAME_INSTANCES[ gameKey ]
			return gameInstance.onQuery( command )

	def onCreateGame( self, command ):
		global GAME_INSTANCES
		if command.params.has_key( "name" ) == False:
			return "NoName"
		if command.params.has_key( "password" ):
			password = command.params[ "password" ]
		else:
			password = ""		
		name = command.params[ "name" ]
		keyString = "%s (%s)" % (name, datetime.datetime.now() )
		m = hashlib.sha256()
		m.update( keyString )
		key = m.hexdigest()
		if GAME_INSTANCES.has_key( key ):
			return "TryAgain"
		GAME_INSTANCES[ key ] = GameInstance( name, password )
		return key

	def onStartGame( self, command ):
		global GAME_INSTANCES
		if command.params.has_key( "gameKey" ) == False:
			return "NoGameKey"
		key = command.params[ "gameKey" ]
		if GAME_INSTANCES.has_key( key ) == False:
			return "TryAgain"
		return GAME_INSTANCES[ key ].onStart( command )

	def onJoinGame( self, command ):
		global GAME_INSTANCES
		if command.params.has_key( "gameKey" ) == False:
			return "NoGameKey"
		key = command.params[ "gameKey" ]
		if GAME_INSTANCES.has_key( key ) == False:
			return "NoGame"
		return GAME_INSTANCES[ key ].onJoin( command )

	def onMoveGame( self, command ):
		global GAME_INSTANCES
		if command.params.has_key( "gameKey" ) == False:
			return "NoGameKey"
		key = command.params[ "gameKey" ]
		if GAME_INSTANCES.has_key( key ) == False:
			return "NoGame"
		return GAME_INSTANCES[ key ].onMove( command )

	def do_GET( self ):
		"""Respond to a GET request."""
		self.send_response( 200 )
		self.send_header( "Content-type", "text/plain" )
		self.end_headers()
		command = HttpCommand( self.path )
		if self.commandHandlers.has_key( command.cmd ):
			handerOutput = self.commandHandlers[ command.cmd ]( command )
			self.wfile.write( "%s:%s" % ( command.cmd, handerOutput ) )

def run( server_class=BaseHTTPServer.HTTPServer,
         handler_class=BaseHTTPServer.BaseHTTPRequestHandler ):
    server_address = ('', 8000)
    httpd = server_class(server_address, handler_class)
    httpd.serve_forever()

run( BaseHTTPServer.HTTPServer, HttpCommandHandler )