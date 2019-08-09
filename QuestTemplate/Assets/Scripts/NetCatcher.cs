using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetCatcher : MonoBehaviour
{
    public Transform target;
    public Transform rimBound;
    public Transform netBound;
    private float linearLimit = 0.02f;

    // Added reference to River manager since we're going to be calling him a lot. Think of it like speed dial.
    private RiverManager rm;

    // Added reference to this objects grabbable component to get grabbable info
    private POVRGrabbable grabbable;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool canSplash = true;

    [ReadOnly]
    public GameObject caughtItem = null;

    private FXManager fx;

    private void Start() 
    {
        fx = FXManager.GetInstance();
        rm = RiverManager.instance;
        grabbable = GetComponent<POVRGrabbable>();

        if (!grabbable)
        {
            Debug.Log("Could not find grabbable component");
        }

        // Save starting position
        startPos = transform.position;
        startRot = transform.rotation;

        if (!rm)
        {
            Debug.Log("No River Manager found, danger Will Robinson");
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        // Prepare to invoke reset when dropped in water
        if (other.gameObject.tag == "Water" && canSplash)
        {
            if (!grabbable.isGrabbed && !grabbable.rb.isKinematic)
                Invoke("Reset", 5f);

            Vector3 splashPos = rimBound.position;
            splashPos[1] = other.transform.position.y;

            fx.Burst(FXManager.FX.Ripple,splashPos, 1); 
            canSplash = false;
        }

        //PhotonView view = grabbable.GetComponent<PhotonView>();
        if (!grabbable.pv.IsMine)
            return;

        if (other.gameObject.tag == "Collectable")
        {

            if (caughtItem != null || other.transform.parent != null)
                return;

            caughtItem = other.gameObject;
            var item = caughtItem.GetComponent<Item>();
            var pv = caughtItem.GetComponent<PhotonView>();
            var rb = caughtItem.GetComponent<Rigidbody>();
            var col = caughtItem.GetComponent<Collider>();

            if (rb)
            {
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            if (col)
            {
                col.isTrigger = true;
            }

            if (pv)
            {
                pv.RequestOwnership();
            }

            caughtItem.transform.SetParent(target);
            caughtItem.transform.localPosition = Vector3.zero;

            // Set the owner of the item
            if (item)
            {
                if ((rm.isHost && transform.root.GetComponent<PandaController>())
                    || !rm.isHost && !transform.root.GetComponent<PandaController>())
                {
                    item.owner = 0;
                }
                else 
                {
                    item.owner = 1;
                }
            }
            else
            {
                Debug.Log("No item component found.");
            }
        }
    }

    private void Reset() 
    {
        if (grabbable.isGrabbed || grabbable.rb.isKinematic)
        {
            // Don't teleport if the player manages to pick it up.
            return;
        }

        transform.position = startPos; 
        transform.rotation = startRot;
    }

    private void Update() 
    {
        if (!grabbable.pv.IsMine)
            return;

        if (caughtItem != null)
        {
            if (!caughtItem.gameObject.activeSelf)
            {
                caughtItem.transform.SetParent(null);
                caughtItem = null;
                return;
            }

            if (caughtItem.transform.parent == null)
            {
                caughtItem.transform.parent = transform;
                caughtItem.transform.localPosition = Vector3.zero;
            }

            // fall out of net
            if (rimBound.position.y - netBound.position.y < linearLimit)
            {
                caughtItem.transform.SetParent(null);


                var rb = caughtItem.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.useGravity = true;
                }

                var col = caughtItem.GetComponent<Collider>();

                if (col)
                {
                    col.isTrigger = false;
                }
                
                caughtItem = null;
            }
        }

    }

    /*private void OnTriggerStay(Collider other) 
    {
        //Instead of comparing rotation with Quaternions, the net is checking a linear ratio between two tranforms located at the net end and the rim end. 
        //By comparing the difference in y postions, the code can determine if the net is in a "dipping up" or "dipping down" position 
        //regardless of axis of rotation and grab the collectable accordingly.
        if (other.gameObject.tag == "Collectable" && rimBound.position.y - netBound.position.y > linearLimit) // Moved the tag lookup first to take advantage of short circuit
        {
            other.gameObject.transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f); 
        }
    }*/

     private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Water" && !canSplash)
        {
            Vector3 splashPos = rimBound.position;
            splashPos[1] = other.transform.position.y;

            fx.Burst(FXManager.FX.Splash, splashPos, 1); 
            canSplash = false;
        }

        if (!RiverManager.instance.isHost)
        {
            return;
        }

        if (other.gameObject.tag == "Collectable")
        {
            var pv = other.GetComponent<PhotonView>();

            if (pv != null)
            {
                pv.TransferOwnership(0);
            }
        }


    }
}