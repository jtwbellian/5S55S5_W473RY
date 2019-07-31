using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class POVRGrabbable : OVRGrabbable
{
    public PhotonView pv = null;

    void Start() {
        base.Start();

        if (pv == null)
            pv =  GetComponent<PhotonView>();
    }

    public override void OnDrop()
    {
        if (pv != null)
        {
            pv.TransferOwnership(0);
        }
    }

    public override void OnGrab()
    {
        if (pv != null)
        {
            pv.RequestOwnership();
        }
    }

}
