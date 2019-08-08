using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonActor : MonoBehaviour
{
    // Start is called before the first frame update
    [PunRPC]
    void RemoveBlock(int BlockToRemove, bool setActive)
    {
        PhotonView Disable = PhotonView.Find(BlockToRemove);
        Disable.transform.gameObject.SetActive(setActive);
    }

    [PunRPC]
    void MoveTo(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
    }

    [PunRPC]
    void Gravity(bool active)
    {
       var rb = transform.GetComponentInChildren<Rigidbody>();
       
       if (rb)
            rb.useGravity = active;
    }

    public void SetGravity(bool active)
    {
        GetComponent<PhotonView>().RPC("Gravity", RpcTarget.AllBuffered, active);
    }

    public void DisableChildObject(bool setActive)
    {
        GetComponent<PhotonView>().RPC("RemoveBlock", RpcTarget.AllBuffered, transform.gameObject.GetComponent<PhotonView>().ViewID, setActive);
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        GetComponent<PhotonView>().RPC("MoveTo", RpcTarget.AllBuffered, pos, rot);
    }

}
