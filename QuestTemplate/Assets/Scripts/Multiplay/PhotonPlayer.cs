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
                                                Vector3.zero, 
                                                Quaternion.identity, 0);
            
            GameObject obj = Resources.Load<GameObject>("PhotonPrefabs/OVRPlayerController");
            var player = Instantiate(obj);
            //player.transform.position =  GameSetup.GS.spawnPoints[GameSetup.GS.currentSpawn].position;

            parts = myAvatar.GetComponent<AvatarParts>();
            avController = player.GetComponent<AvatarController>();
            
            parts.Head.SetParent(avController.headTarget.transform);
            parts.Head.localRotation = Quaternion.identity;
            parts.Head.localPosition = Vector3.zero;

            parts.RHand.SetParent(avController.rhandTarget.transform);
            parts.RHand.localRotation = Quaternion.identity;
            parts.RHand.localPosition = Vector3.zero;

            parts.LHand.SetParent(avController.lhandTarget.transform);
            parts.LHand.localRotation = Quaternion.identity;
            parts.LHand.localPosition = Vector3.zero;

            if (PhotonNetwork.IsMasterClient)
            {
                parts.scarfRed.SetActive(false);
            }
            else
            {
                parts.scarfBlue.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
