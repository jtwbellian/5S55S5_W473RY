using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Transform headTarget = null;
    public Transform lhandTarget = null;
    public Transform rhandTarget = null;

    private Animator rhandPose, lhandPose;

    private float lGrab = 0f;
    private float lFinger = 0f;
    private bool lThumb = false;
    private float rGrab = 0f;
    private float rFinger = 0f;
    private bool rThumb = false;
    [ReadOnly]
    public bool rHoldNet = false;
    [ReadOnly]
    public bool lHoldNet = false;

    private OVRGrabber rGrabber, lGrabber;

    // Start is called before the first frame update
    void Start()
    {
        rhandPose = rhandTarget.GetComponentInChildren<Animator>();
        lhandPose = lhandTarget.GetComponentInChildren<Animator>();

        rhandPose.speed = 1f;
        lhandPose.speed = 1f;

        rGrabber = rhandTarget.GetComponent<OVRGrabber>();
        lGrabber = lhandTarget.GetComponent<OVRGrabber>();
    }

    void Update()
    {
        // Hands disappear for rudder
        POVRGrabbable pgRight = rGrabber.m_grabbedObj as POVRGrabbable;
        POVRGrabbable pgLeft = lGrabber.m_grabbedObj as POVRGrabbable;

        if (pgRight != null && pgRight.hidesHands)
        {
            rhandPose.gameObject.SetActive(false);
        }
        else if (!rhandPose.gameObject.activeSelf)
        {
            rhandPose.gameObject.SetActive(true);
        }

        if (pgLeft != null && pgLeft.hidesHands)
        {
            lhandPose.gameObject.SetActive(false);
        }
        else if (!lhandPose.gameObject.activeSelf)
        {
            lhandPose.gameObject.SetActive(true);
        }


        // Change to check components for more grab poses, right now one pose for all grabbables
        // Right hand hold net
        if (!rHoldNet && rGrabber.m_grabbedObj != null)
        {
            rhandPose.SetBool("HoldNet", true);   
            rHoldNet = true;
        }
        
        if (rHoldNet && rGrabber.m_grabbedObj == null)
        {
            rhandPose.SetBool("HoldNet", false);
            rHoldNet = false;
        }

        // Left hand hold net
         if (!lHoldNet && lGrabber.m_grabbedObj != null)
        {
            lhandPose.SetBool("HoldNet", true);   
            lHoldNet = true;
        }
        
        if (lHoldNet && lGrabber.m_grabbedObj == null)
        {
            lhandPose.SetBool("HoldNet", false);
            lHoldNet = false;
        }

        rGrab = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        rFinger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        rThumb = OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons);

        lGrab = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        lFinger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        lThumb = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons);

        rhandPose.SetFloat("Grip", rGrab);
        rhandPose.SetFloat("Index", rFinger);
        rhandPose.SetBool("ThumbDown", rThumb);

        lhandPose.SetFloat("Grip", lGrab);
        lhandPose.SetFloat("Index", lFinger);
        lhandPose.SetBool("ThumbDown", lThumb);
    }
}
