﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FaceMaker : MonoBehaviour
{
    [SerializeField]
    private Renderer headRenderer;
    [SerializeField]
    private AvatarParts avatarParts;
    
    private PhotonView view;

    public Material [] faceMats;

    private void Start() 
    {
        view = GetComponent<PhotonView>();

        if (avatarParts)
        {
            headRenderer = avatarParts.headRenderer;
        }
    }

    private void Update() 
    {
        if (!view.IsMine)
            return;

        // Facial Gestures
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            view.RPC("RPC_ChangeFace", RpcTarget.AllBuffered, 1);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            view.RPC("RPC_ChangeFace", RpcTarget.AllBuffered, 2);
        }
    }

    public void RevertFace()
    {
        view.RPC("RPC_ChangeFace", RpcTarget.AllBuffered, 0);
    }

    [PunRPC]
    public void RPC_ChangeFace(int face)
    {
        headRenderer.material = faceMats[face];

        if (face != 0)
        {
            CancelInvoke("RevertFace");
            Invoke("RevertFace", 3f);
        }
    }
}
