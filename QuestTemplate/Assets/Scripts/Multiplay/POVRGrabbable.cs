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

    void Start()
    {
        base.Start();

        if (pv == null)
            pv =  GetComponent<PhotonView>();

        if (snapOffset != null)
        {
            Vector3 snapPos;
            Quaternion snapRot;

            snapPos = snapOffset.localPosition;
            snapRot = snapOffset.localRotation;

            // Fixes Oculus's shitty snapOffset system
            snapOffset.SetParent(null);
            snapOffset.position = snapPos;
            snapOffset.rotation = snapRot;
            rb = GetComponentInChildren<Rigidbody>();
        }
    }

    public override void OnDrop()
    {
        if (pv != null)
        {
            //cpv.TransferOwnership(0);
            pv.RPC("SetHeld", RpcTarget.AllBuffered, false);
            //rb.isKinematic = false;
            //isHeld = false;
        }
        
        var fruit = GetComponent<PhotonFruit>();

        if (fruit)
        {
            fruit.rigidBody.isKinematic = false;
            fruit.rigidBody.useGravity = true;
            fruit.isHeld = false;
        }

        var item = GetComponentInChildren<Item>();

        if (item)
            item.transform.SetParent(null);
    }

    public override void OnGrab()
    {
        if (isHeld)
        {
            return;
        }

        if (vibrationClip)
        {
            HapticsManager.Vibrate(vibrationClip, grabbedBy.m_controller);
        }

        var item = GetComponent<Item>();
        
        // Set the owner of the item
        if (item)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                item.owner = 0;
            }
            else 
            {
                item.owner = 1;
            }
        }

        if (pv != null)
        {
            //pv.TransferOwnership(pv.ViewID);
            pv.RequestOwnership();
            pv.RPC("SetHeld", RpcTarget.AllBuffered, true);

            //isHeld = true;
            //rb.isKinematic = true;
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
        m_grabbedBy.ForceRelease(this);
        //transform.SetParent(null);

        if (pv.IsMine && PhotonNetwork.IsMasterClient)
            pv.TransferOwnership(1);
        else
            pv.TransferOwnership(0);
    }
}
