using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class POVRGrabbable : OVRGrabbable
{
    public PhotonView pv = null;
    public Rigidbody rb;

    void Start() {
        base.Start();

        if (pv == null)
            pv =  GetComponent<PhotonView>();

        var snapPos = snapOffset.localPosition;
        var snapRot = snapOffset.localRotation;

        // Fixes Oculus's shitty snapOffset system
        snapOffset.SetParent(null);
        snapOffset.position = snapPos;
        snapOffset.rotation = snapRot;
        rb = GetComponentInChildren<Rigidbody>();
    }

    public override void OnDrop()
    {
        if (pv != null)
        {
            pv.TransferOwnership(0);
            pv.RPC("SetKinematic", RpcTarget.Others, false);
        }
    }

    public override void OnGrab()
    {
        if (pv != null)
        {
            pv.RequestOwnership();
            pv.RPC("SetKinematic", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    public void SetKinematic(bool active)
    {
        rb.isKinematic = active;
    }

}
