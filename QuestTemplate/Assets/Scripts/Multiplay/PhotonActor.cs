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
        Disable.transform.gameObject.SetActive(setActive);
    }

    [PunRPC]
    void MoveTo(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
    }

    [PunRPC]
    void ChildTo(int block, Vector3 pos, Quaternion rot)
    {
        if (block == -1)
        {
            transform.SetParent(null);
            return;
        }

        PhotonView parent = PhotonView.Find(block);

        transform.SetParent(parent.transform);
        transform.localPosition = pos;
        transform.localRotation = rot;

        Debug.Log("Chidling to block " + block.ToString() + " With offset " + pos.ToString());
    }

    [PunRPC]
    void ForcesTo(bool active, Vector3 velocity, Vector3 angVelocity)
    {
       var rb = transform.GetComponentInChildren<Rigidbody>();
       
       if (rb)
       {
            rb.useGravity = active;
            rb.velocity = velocity;
            rb.angularVelocity = angVelocity;
       }
    }


    public void ChildToPhotonTransform(Transform obj, Vector3 pos, Quaternion rot)
    {
        if (obj == null)
            view.RPC("ChildTo", RpcTarget.AllBuffered, -1, pos, rot);
        else
            view.RPC("ChildTo", RpcTarget.AllBuffered, obj.GetComponent<PhotonView>().ViewID, pos, rot);
    }

    public void SetForces(bool active, Vector3 velocity, Vector3 angVelocity)
    {
        view.RPC("ForcesTo", RpcTarget.AllBuffered, active, velocity, angVelocity);
        Debug.Log("Forces set to: " + (active == true ? "Gravity: ON" : "Gravity: OFF") + ", velocity: " + velocity.ToString());
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
