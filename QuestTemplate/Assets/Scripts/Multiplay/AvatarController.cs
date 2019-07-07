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
    private float lThumb = 0f;
    private float rGrab = 0f;
    private float rFinger = 0f;
    private float rThumb = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rhandPose = rhandTarget.GetComponent<Animator>();
        lhandPose = lhandTarget.GetComponent<Animator>();
    }

    void Update()
    {
        rGrab = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        rFinger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        rThumb = (OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons) ? 0f : 1f);

        lGrab = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        lFinger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        lThumb = (OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons) ? 1f : 0f);

        rhandPose.SetFloat("RGrab", rGrab);
        rhandPose.SetFloat("RFinger", rFinger);
        rhandPose.SetFloat("RThumb", rThumb);

        lhandPose.SetFloat("LGrab", lGrab);
        lhandPose.SetFloat("LFinger", lFinger);
        lhandPose.SetFloat("LThumb", lThumb);
    }
}
