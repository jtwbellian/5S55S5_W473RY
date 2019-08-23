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

    private bool inWater = false;
    private RiverManager rm;
    private POVRGrabbable grabbable;

    private Vector3 properLocalOffset;
    private const float TOL = 0.002f; // distance fruit can 'lag' before it is reset locally

    public bool isClam = false; // As disgusted as you are, I'm more disgusted with myself. Had to get it done :\

    // Start is called before the first frame update
    void Start()
    {
        rm = RiverManager.instance;
        sm = SoundManager.instance;
        view = GetComponent<PhotonView>();
        actor = GetComponent<PhotonActor>();
        rigidBody = GetComponent<Rigidbody>();
        fx = FXManager.GetInstance();
        collider = GetComponent<Collider>();
        grabbable = GetComponent<POVRGrabbable>();
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


        if (inWater && isHeld == false && transform.parent == null)
            transform.Translate(rm.riverVelocity + rm.boatRightVelocity, Space.World);

        
        // delete after 20m behind boat
        var behindBoatZ = (rm.boat.transform.position.z - 20f);
        
        if (transform.position.z < behindBoatZ)
        {
            actor.DisableChildObject(false);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.tag == "Water")
        {
            inWater = false;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!view.IsMine) 
            return;

        bool isHung = false;

        if (transform.parent != null)
        {
            if (transform.parent.GetComponent<FruitTree>() != null)
            isHung = true;
        }

        if (!rigidBody.useGravity && isHung && !other.CompareTag("Player")) // Only fall if on tree branch
        {
            PhotonView otherView;
            int id = 0;
            otherView = other.GetComponent<PhotonView>();

            if (otherView)
            {
                id = otherView.ViewID;
                view.TransferOwnership(id);

                if (!isClam)
                    ActivateFruit();
            }
        }

        if (other.gameObject.tag == "Water")
        {
            inWater = true;

            fx.Burst(FXManager.FX.Splash, transform.position, 5);
            fx.Burst(FXManager.FX.Ripple, transform.position -  Vector3.up * 0.3f, 1);
            fx.Burst(FXManager.FX.Spray, transform.position, 2);
            int randomIndex = Random.Range(0, clips.Length - 1);
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
/* 
        if (other.transform.CompareTag("Left"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 3f, Space.World);
        }

        if (other.transform.CompareTag("Right"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * -3f, Space.World);
        }
        */
    }
    
    
    public void ActivateFruit()
    {
        view.RPC("RPC_ActivateFruit", RpcTarget.AllBuffered);
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
        properLocalOffset = positionOffset;
        transform.localRotation = angleOffset;
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        collider.isTrigger = true;

        if (!target.GetComponent<FruitTree>())
        {
            isHeld = true;
            /* if (grabbable)
            {
                if (!isHeld && grabbable.isHeld)
                {
                    isHeld = true;
                }
            }*/
        }
    }

    #region PunRPC

    [PunRPC]
    void RPC_ActivateFruit()
    {
        transform.SetParent(null);
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        collider.isTrigger = false;
        isHeld = false;
    }

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

    #endregion

    public void ChildToPhotonTransform(Transform obj, Vector3 pos, Quaternion rot)
    {
        if (obj == null)
        {
            UnlockFromParent();
            view.RPC("RPC_ChildTo", RpcTarget.Others, -1, pos, rot);
        }
        else
        {
            LockToParent(obj, pos, rot);
            view.RPC("RPC_ChildTo", RpcTarget.Others, obj.GetComponent<PhotonView>().ViewID, pos, rot);
        }
    }
}
