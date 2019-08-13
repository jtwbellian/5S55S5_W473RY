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
    private SoundManager sm;

    // Added reference to this objects grabbable component to get grabbable info
    public POVRGrabbable grabbable;
    private Vector3 startPos;
    private Quaternion startRot;
    private int splashCounter = 0;
    private PhotonView view;

    [ReadOnly]
    public GameObject caughtItem = null;

    private FXManager fx;
    public AudioClip[] enterClips;
    public AudioClip[] exitClips;
    public AudioClip catchClip;

    private void Start() 
    {
        fx = FXManager.GetInstance();
        rm = RiverManager.instance;
        sm = SoundManager.instance;

        if (grabbable == null)
            grabbable = GetComponent<POVRGrabbable>();

        view = GetComponent<PhotonView>();

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
        if (other.gameObject.tag == "Water")
        {
            splashCounter += 1;

            if (!grabbable.isGrabbed && !grabbable.rb.isKinematic)
                Invoke("Reset", 5f);
            
            if (splashCounter == 1 )
            {
                Debug.Log("Splash counter is " + splashCounter);

                Vector3 splashPos = rimBound.position;
                splashPos[1] = other.transform.position.y;

                fx.Burst(FXManager.FX.Ripple,splashPos, 1); 

                //play sound effects
                int randomIndex = Random.Range(0, enterClips.Length);

                sm.PlaySingle(enterClips[randomIndex], transform.position);
                HapticsManager.Vibrate(enterClips[randomIndex], grabbable.grabbedBy.m_controller);
            }
        }


        if (!view.IsMine) // Only do this next part if the view is mine
            return;

        if (other.gameObject.tag == "Collectable")
        {
            if (caughtItem != null || other.transform.parent != null)
                return;

            caughtItem = other.gameObject;
            var item = caughtItem.GetComponent<Item>();
            var fish = caughtItem.GetComponent<PhotonFish>();

            if (fish)
            {
                if(!fish.isHeld)
                {
                    fish.view.TransferOwnership(view.ViewID);
                    fish.ChildToPhotonTransform(transform, target.localPosition, Quaternion.identity);
                    HapticsManager.Vibrate(catchClip, grabbable.grabbedBy.m_controller);
                }
            }

            // Set the owner of the item
            if (item)
            {
                if (rm.isHost)
                {
                    item.owner = 0;
                }
                else 
                {
                    item.owner = 1;
                }
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
        if (!view.IsMine)
            return;

        if (caughtItem != null)
        {
            if (!caughtItem.gameObject.activeSelf) // If still holding and object disabled
            {
                caughtItem.transform.SetParent(null);
                caughtItem = null;
                return;
            }

            if (caughtItem.transform.parent == null)
            {
                caughtItem.transform.parent = transform;
                caughtItem.transform.localPosition = target.localPosition;
            }

            // fall out of net
            if (rimBound.position.y - netBound.position.y < linearLimit)
            {
                var fish = caughtItem.GetComponent<PhotonFish>();

                if (fish)
                {
                    fish.ChildToPhotonTransform(null, Vector3.zero, Quaternion.identity);
                }
                
                caughtItem = null;
            }
        }
    }


     private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Water")          //Testing a better method to detect enter water
        {
            splashCounter -= 1;

            if (splashCounter == 0)
            {
                Vector3 splashPos = rimBound.position;
                splashPos[1] = other.transform.position.y;

                fx.Burst(FXManager.FX.Splash, splashPos, 1); 

                //play sound effects
                int randomIndex = Random.Range(0, exitClips.Length);
                sm.PlaySingle(exitClips[randomIndex], transform.position);
            }
        }
    }
}

