﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks 
{
    // Room info
    private static PhotonRoom room;
    private PhotonView PV;

    public int multiplayerScene;

    // Player Info
    Player[] photonPlayers;
    private int playersInRoom;
    private int myNumberInRoom;
    private int currentScene = 0;
    private int playerInGame;

    private void Awake() 
    {
        PV = GetComponent<PhotonView>();

        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                PhotonNetwork.Destroy(PhotonRoom.room.GetComponent<PhotonView>());
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    } 

    public override void OnEnable() 
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    public override void OnDisable() 
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName =  "Player " + myNumberInRoom.ToString();
        StartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room!");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
    }

    void StartGame()
    {
        if (! PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == multiplayerScene)
        {
            CreatePlayer();
            playerInGame++;
        }
    }

    // Creates a new PhotonNetwork Player who will spawn the avatar parts and the player controller
    void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), new Vector3(0, 4f, -1.5f), Quaternion.identity, 0);
    }
}