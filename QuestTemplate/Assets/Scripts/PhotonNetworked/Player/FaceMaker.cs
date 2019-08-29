using System.Collections;
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
        if (!view.IsMine) // Only check for input on local side
            return;

        // Facial Gestures
        if (OVRInput.GetDown(OVRInput.Button.One)) // Bad I know
        {
            view.RPC("RPC_ChangeFace", RpcTarget.AllBuffered, 1);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two)) // I'm more disappointed in myself really
        {
            view.RPC("RPC_ChangeFace", RpcTarget.AllBuffered, 2);
        }
    }

    public void RevertFace()
    {
        view.RPC("RPC_ChangeFace", RpcTarget.AllBuffered, 0);
    }

    /// <summary>
    /// RPC function so the panda face is updated on both ends
    /// </summary>
    [PunRPC]
    public void RPC_ChangeFace(int face)
    {
        headRenderer.material = faceMats[face];

        if (face != 0)
        {
            CancelInvoke("RevertFace");
            Invoke("RevertFace", 3f); // Return to neutral face 
        }
    }
}
