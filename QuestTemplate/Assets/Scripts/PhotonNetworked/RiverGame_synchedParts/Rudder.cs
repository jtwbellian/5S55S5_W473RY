﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Rudder : POVRGrabbable
{
    public Transform targetObject;
    public Vector3 localStartPos;
    public Vector3 startPos;
    public Quaternion localStartRot;
    public GameObject leftHand;
    public GameObject rightHand;
    public Boat boat;

    private float maxAngle = 20f;
    private float minAngle = -20f;
    private float returnSpeed = 0.01f;

    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        localStartPos = transform.localPosition;
        localStartRot = transform.localRotation;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            if (transform.position.z < targetObject.position.z)
               return;

           Vector3  newTarget = new Vector3(transform.position.x, targetObject.transform.position.y, transform.position.z);
           targetObject.transform.LookAt(newTarget);
           //Debug.Log("Rudder Euler: " + targetObject.transform.rotation.eulerAngles);
            //RiverManager.instance.AddForceToRudder(Mathf.Clamp(rudderAngle, minAngle, maxAngle) / 10f);
        }
        else if (transform.position != startPos)//(targetObject.transform.rotation != Quaternion.identity)
        {   
            transform.position = startPos;
            targetObject.transform.rotation = Quaternion.identity;//Quaternion.Lerp(targetObject.transform.rotation, Quaternion.identity, Time.time * returnSpeed);       
        }

        // Only modify boat on one client
        if (pv.IsMine)
        {
            var rudderAngle = targetObject.transform.rotation.eulerAngles.y;

            if (rudderAngle > 180)
                rudderAngle -= 360;

            if (rudderAngle < -180)
                rudderAngle += 360;

            if (Mathf.Abs(rudderAngle) > 5f)
                RiverManager.instance.MoveRudder(-Mathf.Clamp(rudderAngle, minAngle, maxAngle) / 700f);  
        }
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);

        // right hand
        if (hand.m_controller == OVRInput.Controller.RTouch)
        {   
            rightHand.GetComponent<MeshRenderer>().sharedMaterial = hand.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material;
            rightHand.SetActive(true);
        }
        else // left hand
        {
            leftHand.GetComponent<MeshRenderer>().sharedMaterial = hand.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material;
            leftHand.SetActive(true);
        }

        // Request ownership
        if (pv != null)
        {
            pv.RequestOwnership();
            pv.RPC("SetHeld", RpcTarget.AllBuffered, true);
        }
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(Vector3.zero, Vector3.zero);  
        //transform.position = startPos;

        //transform.SetParent(targetObject);
        //transform.localPosition = localStartPos;
        //transform.localRotation = localStartRot;

        leftHand.SetActive(false);
        rightHand.SetActive(false);

        pv.RPC("SetHeld", RpcTarget.AllBuffered, false);
    }
}