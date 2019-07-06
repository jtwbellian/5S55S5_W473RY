using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    public static PhotonLobby lobby;

    public GameObject PlayButton;

    public GameObject CancelButton;

    private int maxPlayers = 2;

private void Awake() {
    {
        lobby = this; // quick singleton implementation
    }
}

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // try to connect to photon master server
    }

#region  Networking Functions 
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the Photon Master Server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PlayButton.SetActive(true);
    }
/* 
    public override void OnJoinedRoom()
    {
        Debug.Log("You have joined the room.");
    }
    
*/

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("No games found.");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Trying again...");
        CreateRoom();
    }
    void CreateRoom()
    {
        int ranRoom = Random.Range(0, 1000);
        RoomOptions roomOps = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayers};

        PhotonNetwork.CreateRoom("Room" + ranRoom, roomOps);
        Debug.Log("Trying to create new Room " + ranRoom);
    }

#endregion

    public void OnPlayButtonPressed()
    {
        PlayButton.SetActive(false);
        CancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

public void OnCancelButtonPressed()
{
    CancelButton.SetActive(false);
    PlayButton.SetActive(true);
    PhotonNetwork.LeaveRoom();
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
