using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonFish : MonoBehaviour
{
    public PhotonActor actor;
    public PhotonView view;
    public Rigidbody rigidBody;
    public bool isHeld = false;
    public Collider collider; 
    private Animator animator;
    private FXManager fx;
    private float speed;
    public bool canSwim = true;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        actor = GetComponent<PhotonActor>();
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        fx = FXManager.GetInstance();
        collider = GetComponent<Collider>();

        speed = Random.Range(0.7f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine)
            return;

        if (!isHeld && canSwim)
        {
            transform.Translate(Vector3.forward * -speed * Time.deltaTime, Space.World);
            //rigidBody.useGravity = false;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {

        if (other.gameObject.tag == "Water")
        {
            fx.Burst(FXManager.FX.Splash, transform.position, 5);
            fx.Burst(FXManager.FX.Ripple, transform.position -  Vector3.up * 0.3f, 1);
            fx.Burst(FXManager.FX.Spray, transform.position, 2);

            if (!view.IsMine)  // This will also ensure only one player makes the disable object RPC call
                return;

            animator.SetBool("Flop", false);
            canSwim = true;
            transform.rotation = Quaternion.identity;
            rigidBody.angularVelocity = Vector3.zero;
        }
        // Reach end
        if (other.transform.CompareTag("Finish") || other.transform.CompareTag("Right") || other.transform.CompareTag("Left"))
        {
            actor.DisableChildObject(false);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (!view.IsMine) 
            return;

        if (other.gameObject.tag == "Water") 
        {
            animator.SetBool("Flop", true);  
            canSwim = false;
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
        canSwim = false;

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
