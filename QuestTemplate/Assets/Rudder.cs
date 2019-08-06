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

    private float maxAngle = 35f;
    private float minAngle = -35f;

    [SerializeField]
    private bool isHeld = false;

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
        pv.RPC("SetHeld", RpcTarget.AllBuffered, true);
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);  

        isHeld = false;

        transform.SetParent(targetObject);
        transform.localPosition = localStartPos;
        transform.localRotation = localStartRot;
        pv.RPC("SetHeld", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    void SetHeld(bool h)
    {
        isHeld = h;
    }
}
