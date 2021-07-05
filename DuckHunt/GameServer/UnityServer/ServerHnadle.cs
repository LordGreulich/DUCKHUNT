﻿using NiloxUniversalLib.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Log.Info($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Log.Info($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            Server.clients[_fromClient].SendIntoGame(_username);
        }

        public static void PlayerTransform(int _fromClient, Packet packet)
        {
            Vector3 position = packet.ReadVector3();
            Quaternion quaternion = packet.ReadQuaternion();

            Server.clients[_fromClient].player.updateTransform(position, quaternion);
        }

        public static void damagePlayer(int fromclient, Packet packet)
        {
            int instigatorid = packet.ReadInt();
            int targetid = packet.ReadInt();
            int ammount = packet.ReadInt();

            if (fromclient != instigatorid)
            {
                Log.Warning("Instigator and fromclient didnt match!!!");
                return;
            }

            Server.clients[targetid].player.damage(ammount);
        }
    }
}