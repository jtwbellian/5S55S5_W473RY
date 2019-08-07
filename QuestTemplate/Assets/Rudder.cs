using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Rudder : POVRGrabbable
{
    public Transform targetObject;
    public Vector3 localStartPos;
    public Quaternion localStartRot;
    public GameObject leftHand;
    public GameObject rightHand;

    private float maxAngle = 35f;
    private float minAngle = -35f;

    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        localStartPos = transform.localPosition;
        localStartRot = transform.localRotation;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            //if (transform.position.x > targetObject.position.x)
             //   return;

           Vector3  newTarget = new Vector3(transform.position.x, targetObject.transform.position.y, transform.position.z);
           targetObject.transform.LookAt(newTarget);
        }
    }

     public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);

        // right hand
        if (hand.m_controller == OVRInput.Controller.RTouch)
        {   
            rightHand.SetActive(true);
        }
        else // left hand
        {
            leftHand.SetActive(true);
        }
        //pv.RPC("SetHeld", RpcTarget.AllBuffered, true);
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(Vector3.zero, Vector3.zero);  

        //isHeld = false;

        transform.SetParent(targetObject);
        transform.localPosition = localStartPos;
        transform.localRotation = localStartRot;

        leftHand.SetActive(false);
        rightHand.SetActive(false);
        //pv.RPC("SetHeld", RpcTarget.AllBuffered, false);
    }


}
