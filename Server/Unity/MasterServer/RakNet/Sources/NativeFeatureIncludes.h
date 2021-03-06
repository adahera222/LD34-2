// If you want to change these defines, put them in NativeFeatureIncludesOverrides so your changes are not lost when updating RakNet
// The user should not edit this file
#include "NativeFeatureIncludesOverrides.h"

#ifndef __NATIVE_FEATURE_INCLDUES_H
#define __NATIVE_FEATURE_INCLDUES_H

#ifndef _RAKNET_SUPPORT_AutoRPC
#define _RAKNET_SUPPORT_AutoRPC 0
#endif
#ifndef _RAKNET_SUPPORT_ConnectionGraph
#define _RAKNET_SUPPORT_ConnectionGraph 0
#endif
#ifndef _RAKNET_SUPPORT_ConnectionGraph2
#define _RAKNET_SUPPORT_ConnectionGraph2 0
#endif
#ifndef _RAKNET_SUPPORT_DirectoryDeltaTransfer
#define _RAKNET_SUPPORT_DirectoryDeltaTransfer 0
#endif
#ifndef _RAKNET_SUPPORT_FileListTransfer
#define _RAKNET_SUPPORT_FileListTransfer 0
#endif
#ifndef _RAKNET_SUPPORT_FullyConnectedMesh
#define _RAKNET_SUPPORT_FullyConnectedMesh 0
#endif
#ifndef _RAKNET_SUPPORT_FullyConnectedMesh2
#define _RAKNET_SUPPORT_FullyConnectedMesh2 0
#endif
#ifndef _RAKNET_SUPPORT_LightweightDatabaseClient
#define _RAKNET_SUPPORT_LightweightDatabaseClient 1
#endif
#ifndef _RAKNET_SUPPORT_LightweightDatabaseServer
#if UNITY_MASTERSERVER
#define _RAKNET_SUPPORT_LightweightDatabaseServer 1
#else
#define _RAKNET_SUPPORT_LightweightDatabaseServer 0
#endif
#endif
#ifndef _RAKNET_SUPPORT_MessageFilter
#define _RAKNET_SUPPORT_MessageFilter 0
#endif
#ifndef _RAKNET_SUPPORT_NatPunchthroughClient
#define _RAKNET_SUPPORT_NatPunchthroughClient 1
#endif
#ifndef _RAKNET_SUPPORT_NatPunchthroughServer
#if UNITY_FACILITATOR
#define _RAKNET_SUPPORT_NatPunchthroughServer 1
#else
#define _RAKNET_SUPPORT_NatPunchthroughServer 0
#endif
#endif
#ifndef _RAKNET_SUPPORT_NatTypeDetectionClient
#define _RAKNET_SUPPORT_NatTypeDetectionClient 1
#endif
#ifndef _RAKNET_SUPPORT_NatTypeDetectionServer
#if UNITY_CONNECTIONTESTER
#define _RAKNET_SUPPORT_NatTypeDetectionServer 1
#endif
#else
#define _RAKNET_SUPPORT_NatTypeDetectionServer 0
#endif
#ifndef _RAKNET_SUPPORT_PacketLogger
#define _RAKNET_SUPPORT_PacketLogger 1
#endif
#ifndef _RAKNET_SUPPORT_ReadyEvent
#define _RAKNET_SUPPORT_ReadyEvent 0
#endif
#ifndef _RAKNET_SUPPORT_ReplicaManager
#define _RAKNET_SUPPORT_ReplicaManager 0
#endif
#ifndef _RAKNET_SUPPORT_ReplicaManager2
#define _RAKNET_SUPPORT_ReplicaManager2 0
#endif
#ifndef _RAKNET_SUPPORT_ReplicaManager3
#define _RAKNET_SUPPORT_ReplicaManager3 0
#endif
#ifndef _RAKNET_SUPPORT_Router
#define _RAKNET_SUPPORT_Router 0
#endif
#ifndef _RAKNET_SUPPORT_Router2
#define _RAKNET_SUPPORT_Router2 0
#endif
#ifndef _RAKNET_SUPPORT_RPC4Plugin
#define _RAKNET_SUPPORT_RPC4Plugin 0
#endif
#ifndef _RAKNET_SUPPORT_TeamBalancer
#define _RAKNET_SUPPORT_TeamBalancer 0
#endif
#ifndef _RAKNET_SUPPORT_UDPProxyClient
#define _RAKNET_SUPPORT_UDPProxyClient 0
#endif
#ifndef _RAKNET_SUPPORT_UDPProxyCoordinator
#define _RAKNET_SUPPORT_UDPProxyCoordinator 0
#endif
#ifndef _RAKNET_SUPPORT_UDPProxyServer
#define _RAKNET_SUPPORT_UDPProxyServer 0
#endif
#ifndef _RAKNET_SUPPORT_ConsoleServer
#define _RAKNET_SUPPORT_ConsoleServer 0
#endif
#ifndef _RAKNET_SUPPORT_RakNetTransport
#define _RAKNET_SUPPORT_RakNetTransport 0
#endif
#ifndef _RAKNET_SUPPORT_TelnetTransport
#define _RAKNET_SUPPORT_TelnetTransport 0
#endif
#ifndef _RAKNET_SUPPORT_TCPInterface
#define _RAKNET_SUPPORT_TCPInterface 0
#endif
#ifndef _RAKNET_SUPPORT_LogCommandParser
#define _RAKNET_SUPPORT_LogCommandParser 0
#endif
#ifndef _RAKNET_SUPPORT_RakNetCommandParser
#define _RAKNET_SUPPORT_RakNetCommandParser 0
#endif

#if _RAKNET_SUPPORT_TCPInterface==0
#ifndef _RAKNET_SUPPORT_EmailSender
#define _RAKNET_SUPPORT_EmailSender 0
#endif
#ifndef _RAKNET_SUPPORT_HTTPConnection
#define _RAKNET_SUPPORT_HTTPConnection 0
#endif
#ifndef _RAKNET_SUPPORT_PacketizedTCP
#define _RAKNET_SUPPORT_PacketizedTCP 0
#endif
#endif

#endif // __NATIVE_FEATURE_INCLDUES_H
