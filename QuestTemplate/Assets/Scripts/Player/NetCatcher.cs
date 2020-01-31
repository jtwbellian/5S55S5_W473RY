﻿using System.Collections;
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
            Debug.Log("No River Manager found");
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
                int randomIndex = Random.Range(0, enterClips.Length - 1);

                sm.PlaySingle(enterClips[randomIndex], transform.position);
                
                if (grabbable.grabbedBy)
                    HapticsManager.Vibrate(enterClips[randomIndex], grabbable.grabbedBy.m_controller);
            }
        }

        if (!view || !view.IsMine || grabbable.grabbedBy == null) // Only do this next part if the view is mine
            return;

        if (other.gameObject.tag == "Collectable")
        {
            // Janky ass code to allow me to catch fruit from trees
            var canCatch = true;

            // Do not catch if item already caught or if childed to a non-fruit tree object
            if (caughtItem != null || (other.transform.parent != null && !other.transform.parent.GetComponent<FruitTree>()))
            {
                canCatch = false;
            }

            // Do not catch if net upside down
            /* if (rimBound.position.y - netBound.position.y > linearLimit)
            {
                canCatch = false;
            }*/

            if (!canCatch)
                return;

            caughtItem = other.gameObject;

            var item = caughtItem.GetComponent<Item>();

            var fish = caughtItem.GetComponent<PhotonFish>();
            var fruit = caughtItem.GetComponent<PhotonFruit>();

            // Catch Fish
            if (fish)
            {
                if(!fish.isHeld)
                {
                    if (!fish.view.IsMine)
                        fish.view.RequestOwnership();
                    //fish.view.TransferOwnership(view.ViewID);
                    fish.ChildToPhotonTransform(transform, target.localPosition, Quaternion.identity);
                    HapticsManager.Vibrate(catchClip, grabbable.grabbedBy.m_controller);
                    fish.isHeld = true;
                }
            }

            // Catch Fruits (these should really be children of a parent class, but whatever)
            if (fruit)
            {
                if(!fruit.isHeld)
                {
                    if (!fruit.view.IsMine)
                        fruit.view.RequestOwnership();
                    //fruit.view.TransferOwnership(view.ViewID);
                    fruit.ChildToPhotonTransform(transform, target.localPosition, Quaternion.identity);
                    HapticsManager.Vibrate(catchClip, grabbable.grabbedBy.m_controller);
                    fruit.isHeld = true;
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
        if (!view.IsMine || caughtItem == null)
            return;

        if (caughtItem != null && caughtItem.transform.parent == null)
        {
            caughtItem = null;
        }

        // fall out of net
        if (rimBound.position.y - netBound.position.y < linearLimit)
        {
            var fish = caughtItem.GetComponent<PhotonFish>();

            if (fish)
            {   
                fish.ChildToPhotonTransform(null, Vector3.zero, Quaternion.identity);
            }

            var fruit = caughtItem.GetComponent<PhotonFruit>();

            if (fruit)
            {
                fruit.ChildToPhotonTransform(null, Vector3.zero, Quaternion.identity);
            }
            
            caughtItem = null;
            //Debug.Log("Fish: " + fish.ToString() + ", Fruit: " + fruit.ToString());
        }
/*
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
        }*/
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
