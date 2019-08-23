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
    private const float TOL = 0.002f; // distance fish can 'lag' before it is reset locally
    private Vector3 properLocalOffset;

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

        if (isHeld)
        {
            if (transform.parent == null) // if somehow ends up not held, reset flag
                UnlockFromParent();

            if (Vector3.Distance(transform.localPosition, properLocalOffset) > TOL)
                transform.localPosition = properLocalOffset;
        }

        if (!isHeld && canSwim)
        {
            transform.Translate(Vector3.forward * -speed * Time.deltaTime, Space.World);
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        // delete after 20m behind boat
        var behindBoatZ = (RiverManager.instance.boat.transform.position.z - 20f);
        
        if (transform.position.z < behindBoatZ)
        {
            actor.DisableChildObject(false);
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
        if (other.transform.CompareTag("Finish"))
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
    void RPC_ChildTo(int block, Vector3 pos, Quaternion rot)
    {
        if (block == -1)
        {
            UnlockFromParent();
            return;
        }

        PhotonView parent = PhotonView.Find(block);
        LockToParent(parent.transform, pos, rot);
    }

    [PunRPC]
    void RPC_ResetToSpawn()
    {
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        isHeld = false;
        collider.isTrigger = false;
        Debug.Log("Fish reset");
    }

    public void ResetSpawnSettings()
    {
        view.RPC("RPC_ResetToSpawn", RpcTarget.AllBuffered);
    }

    public void UnlockFromParent()
    {
        Debug.Log("Unlocking From Parent");
        transform.SetParent(null);
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        isHeld = false;
        collider.isTrigger = false;
    }

    public void LockToParent(Transform target, Vector3 positionOffset, Quaternion angleOffset)
    {
        transform.SetParent(target);
        transform.localPosition = positionOffset;
        transform.localRotation = angleOffset;
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        collider.isTrigger = true;
        isHeld = true;
        canSwim = false;

        properLocalOffset = positionOffset;
    }

    #endregion

    public void ChildToPhotonTransform(Transform obj, Vector3 pos, Quaternion rot)
    {
        if (obj == null)
        {
            //UnlockFromParent();
            view.RPC("RPC_ChildTo", RpcTarget.AllBuffered, -1, pos, rot);
        }
        else
        {
            //LockToParent(obj, pos, rot);
            view.RPC("RPC_ChildTo", RpcTarget.AllBuffered, obj.GetComponent<PhotonView>().ViewID, pos, rot);
        }
    }
}
