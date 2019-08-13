using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class POVRGrabbable : OVRGrabbable
{
    public PhotonView pv = null;
    public Rigidbody rb;
    public bool isHeld = false;
    AudioClip vibrationClip;

    public bool hidesHands = false;

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
            //cpv.TransferOwnership(0);
            pv.RPC("SetHeld", RpcTarget.Others, false);
            isHeld = false;
        }
        
        var item = GetComponentInChildren<Item>();

        if (item)
            item.transform.SetParent(null);
    }

    public override void OnGrab()
    {
        if (vibrationClip)
        {
            HapticsManager.Vibrate(vibrationClip, grabbedBy.m_controller);
        }

        if (pv != null)
        {
            pv.RequestOwnership();
            pv.RPC("SetHeld", RpcTarget.Others, true);
            
            if (isHeld)
                pv.RPC("Take", RpcTarget.Others);

            isHeld = true;
        }
    }

    [PunRPC]
    public void SetHeld(bool active)
    {
        rb.isKinematic = active;
        isHeld = active;
    }
    
    [PunRPC]
    public void Take()
    {
        if (isHeld)
        {
            m_grabbedBy.ForceRelease(this);

            if (pv.IsMine && PhotonNetwork.IsMasterClient)
                pv.TransferOwnership(1);
            else
                pv.TransferOwnership(0);
        }
    }
}
