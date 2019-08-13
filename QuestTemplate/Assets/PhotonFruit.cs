using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonFruit : MonoBehaviour
{
    public PhotonActor actor;
    public PhotonView view;
    public Rigidbody rigidBody;
    public bool isHeld = false;
    public Collider collider; 
    private FXManager fx;
    public AudioClip[] clips;
    private SoundManager sm;

    // Start is called before the first frame update
    void Start()
    {
        sm = SoundManager.instance;
        view = GetComponent<PhotonView>();
        actor = GetComponent<PhotonActor>();
        rigidBody = GetComponent<Rigidbody>();
        fx = FXManager.GetInstance();
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine)
            return;
    }

    private void OnTriggerEnter(Collider other) 
    {

        if (other.gameObject.tag == "Water")
        {
            fx.Burst(FXManager.FX.Splash, transform.position, 5);
            fx.Burst(FXManager.FX.Ripple, transform.position -  Vector3.up * 0.3f, 1);
            fx.Burst(FXManager.FX.Spray, transform.position, 2);
            int randomIndex = Random.Range(0, clips.Length);
            sm.PlaySingle(clips[randomIndex], transform.position);

            if (!view.IsMine)  // This will also ensure only one player makes the disable object RPC call
                return;

            transform.rotation = Quaternion.identity;
            rigidBody.angularVelocity = Vector3.zero;
        }
        // Reach end
        if (other.transform.CompareTag("Finish")) //|| other.transform.CompareTag("Right") || other.transform.CompareTag("Left"))
        {
            actor.DisableChildObject(false);
        }
    }

    #region PunRPC

    [PunRPC]
    void ChildTo(int block, Vector3 pos, Quaternion rot)
    {
        if (block == -1)
        {
            transform.SetParent(null);
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;
            isHeld = false;
            collider.isTrigger = false;
            return;
        }

        PhotonView parent = PhotonView.Find(block);

        transform.SetParent(parent.transform);
        transform.localPosition = pos;
        transform.localRotation = rot;
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        collider.isTrigger = true;
        isHeld = true;

        Debug.Log("Chidling to block " + block.ToString() + " With offset " + pos.ToString());
    }


    #endregion

    public void ChildToPhotonTransform(Transform obj, Vector3 pos, Quaternion rot)
    {
        if (obj == null)
            view.RPC("ChildTo", RpcTarget.AllBuffered, -1, pos, rot);
        else
            view.RPC("ChildTo", RpcTarget.AllBuffered, obj.GetComponent<PhotonView>().ViewID, pos, rot);
    }

}
