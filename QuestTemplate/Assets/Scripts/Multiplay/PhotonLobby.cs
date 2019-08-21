using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    public static PhotonLobby lobby;
    private bool gameStarting = false;

    public GameObject PlayButton;

    public GameObject CancelButton;
    public TextMeshProUGUI messageField;

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
        messageField.text = "Connected.";
        PhotonNetwork.AutomaticallySyncScene = true;
        PlayButton.SetActive(true);
    }

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
        Debug.Log("Trying to create new Room " + ranRoom);
        RoomOptions roomOps = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayers};
        PhotonNetwork.CreateRoom("Room" + ranRoom, roomOps);
    }

#endregion

    public void OnMultiPlayPressed()
    {
        if (gameStarting)
            return;
        //PlayButton.SetActive(false);
        CancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        gameStarting = true;
    }

    public void OnSoloPlayPressed()
    {
        if (gameStarting)
            return;
        //PlayButton.SetActive(false);
        //CancelButton.SetActive(true);
        CreateRoom();
        gameStarting = true;
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
