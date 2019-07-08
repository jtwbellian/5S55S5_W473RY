using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Transform headTarget = null;
    public Transform lhandTarget = null;
    public Transform rhandTarget = null;

    private Animator rhandPose, lhandPose;
    private bool leftyMode = false;

    private float lGrab = 0f;
    private float lFinger = 0f;
    private bool lThumb = false;
    private float rGrab = 0f;
    private float rFinger = 0f;
    private bool rThumb = false;

    // Start is called before the first frame update
    void Start()
    {
        rhandPose = rhandTarget.GetComponentInChildren<Animator>();
        lhandPose = lhandTarget.GetComponentInChildren<Animator>();
        rhandPose.speed = 1f;
        lhandPose.speed = 1f;
    }

    void Update()
    {
        rGrab = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        rFinger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        rThumb = OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons);

        lGrab = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        lFinger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        lThumb = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons);

        print("lthumb = " + lThumb + ", rthumb = " + rThumb);

        rhandPose.SetFloat("Grip", rGrab);
        rhandPose.SetFloat("Index", rFinger);
        rhandPose.SetBool("ThumbDown", rThumb);

        lhandPose.SetFloat("Grip", lGrab);
        lhandPose.SetFloat("Index", lFinger);
        lhandPose.SetBool("ThumbDown", lThumb);
    }
}
