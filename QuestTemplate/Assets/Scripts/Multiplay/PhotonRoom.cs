using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks 
{

    // Room info
    public static PhotonRoom room;

    public GameObject [] spawnPoints; 
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
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>();
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

    // Start is called before the first frame update
    void Start()
    {
        /*
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
        */
        //PhotonNetwork.LoadLevel(multiplayerScene);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName =  "Player " + myNumberInRoom.ToString();
        
        /*/
        if (MultiplayerSetting.multiplayerSetting.delayStart)
        {
            Debug.Log(("Players in room: " + playersInRoom.ToString() + " / " + MultiplayerSetting.multiplayerSetting.maxPlayers + " MAX "));
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }

            if (playersInRoom == MultiplayerSetting.multiplayerSetting.maxPlayers)
            {
                readyToStart = true;
                
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            
        }
        else
        {*/
        StartGame();
        //}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room!");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

        /*if (MultiplayerSetting.multiplayerSetting.delayStart)
        {
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }

            if (playersInRoom == MultiplayerSetting.multiplayerSetting.maxPlayers)
            {
                readyToStart = true;
                
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        if (MultiplayerSetting.multiplayerSetting.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                //Debug.Log("Ready to start in... " + timeToStart.ToString());
                if(timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }*/
    }

    void StartGame()
    {
        //isGameLoaded = true;

        if (! PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    void RestartTimer()
    {
        //lessThanMaxPlayers = startingTime;
        //timeToStart = startingTime;
        //atMaxPlayer = 6;
        //readyToCount = false;
        //readyToCount = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == multiplayerScene)
        {
            CreatePlayer();
            playerInGame++;
            /*isGameLoaded = true;
            if (MultiplayerSetting.multiplayerSetting.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }*/
        }
    }

/* 
    [PunRPC]
    void RPC_LoadedGameScene()
    {
        playerInGame ++;
        if (playerInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }
*/

    //[PunRPC]
    void CreatePlayer()
    {
        var currentSpawn = spawnPoints[playerInGame].transform.position;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), currentSpawn, Quaternion.identity, 0);
    }
}
