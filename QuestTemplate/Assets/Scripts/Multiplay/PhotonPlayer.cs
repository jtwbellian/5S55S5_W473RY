﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Photon;


public class PhotonPlayer : MonoBehaviour
{
    PhotonView PV;
    public GameObject myAvatar;
    public AvatarController avController;
    private AvatarParts parts;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Debug.Log("PV set to " + PV);

        if (PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                                                GameSetup.GS.spawnPoints[GameSetup.GS.currentSpawn].position, 
                                                GameSetup.GS.spawnPoints[GameSetup.GS.currentSpawn].rotation, 0);
            GameObject obj = Resources.Load<GameObject>("PhotonPrefabs/OVRPlayerController");
            var player = Instantiate(obj);
            player.transform.position = myAvatar.transform.position;

            parts = myAvatar.GetComponent<AvatarParts>();
            avController = player.GetComponent<AvatarController>();
            
            parts.Head.SetParent(avController.headTarget.transform);
            parts.Head.localRotation = Quaternion.identity;

            parts.RHand.SetParent(avController.rhandTarget.transform);
            parts.RHand.localRotation = Quaternion.identity;

            parts.LHand.SetParent(avController.lhandTarget.transform);
            parts.LHand.localRotation = Quaternion.identity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
