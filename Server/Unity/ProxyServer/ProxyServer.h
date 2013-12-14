#pragma once
#include "RakNetTypes.h"
#include "MessageIdentifiers.h"

#define PROXY_SERVER_PROTOCOL_VERSION 2
#define PROXY_SERVER_VERSION "2.0.0b1"

enum {
	ID_STATE_UPDATE = ID_USER_PACKET_ENUM,
	ID_STATE_INITIAL,
	ID_CLIENT_INIT,
	ID_REMOVE_RPCS,
	ID_REQUEST_CLIENT_INIT,
	ID_PROXY_INIT_MESSAGE,
	ID_PROXY_CLIENT_MESSAGE,
	ID_PROXY_SERVER_MESSAGE,
	ID_PROXY_MESSAGE,
	ID_PROXY_SERVER_INIT
};


struct RelayItem
{
	char* packet;
	int length;
	SystemAddress target;
};
