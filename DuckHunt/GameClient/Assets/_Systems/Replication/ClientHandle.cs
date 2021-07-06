﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"(WELCOME)Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {

        int _id = _packet.ReadInt();
        string username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        _position = Gamemode.instance.spawnlocation;

        Debug.Log($"Received spawn Player {username} at {_position}");

        GameManager.instance.SpawnPlayer(_id, username, _position, _rotation);
    }


    public static void PlayerTransform(Packet _packet)
    {
        try
        {
            int _id = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            if (GameManager.players[_id] == null)
            {
                Debug.LogError("Player with id " + _id + " doesnt exist");
            }

            GameManager.players[_id].transform.rotation = _rotation;
            GameManager.players[_id].transform.position = _position;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        
    }

    public static void updateHealthOfId(Packet packet)
    {
        int id = packet.ReadInt();
        int value = packet.ReadInt();

        GameManager.players[id].GetComponent<Health>().health = value;
        Debug.Log($"Player {GameManager.players[id].username} took {value} Damage");
    }

    public static void playerdied(Packet packet)
    {
        int id = packet.ReadInt();
        int instigatorid = packet.ReadInt();


        Debug.Log($"{GameManager.players[instigatorid].username} killed {GameManager.players[id].username}");

        GameManager.players[id].gameObject.transform.position = new Vector3(0,0,0);

        if (Client.instance.myId == id)
        {
            PlayerController.instance.enabled = false;
                
        }
    }

    public static void playerRespawn(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.players[id].gameObject.transform.position = Gamemode.instance.spawnlocation;

        if (Client.instance.myId == id)
        {
            PlayerController.instance.enabled = true;
        }
    }
}