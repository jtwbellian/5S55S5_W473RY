using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonActor : MonoBehaviour
{
    public PhotonView view;

    private void Awake() 
    {
        view = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    [PunRPC]
    void RemoveBlock(int BlockToRemove, bool setActive)
    {
        PhotonView Disable = PhotonView.Find(BlockToRemove);
        Disable.transform.SetParent(null);
        Disable.transform.gameObject.SetActive(setActive);
    }

    [PunRPC]
    void MoveTo(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
    }

    [PunRPC]
    void ForcesTo(bool grav, Vector3 vel, Vector3 angVel)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb)
        {
            rb.velocity = vel;
            rb.angularVelocity = angVel;
            rb.useGravity = grav;
        }
    }


    public void SetForces(bool grav, Vector3 vel, Vector3 angVel)
    {
        view.RPC("ForcesTo", RpcTarget.AllBuffered, grav, vel, angVel);
    }

    public void DisableChildObject(bool setActive)
    {
        view.RPC("RemoveBlock", RpcTarget.AllBuffered, transform.gameObject.GetComponent<PhotonView>().ViewID, setActive);
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        view.RPC("MoveTo", RpcTarget.AllBuffered, pos, rot);
    }

    public void SetLocalPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        view.RPC("MoveLocalTo", RpcTarget.AllBuffered, pos, rot);
    }

}
