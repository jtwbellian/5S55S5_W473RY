using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : OVRGrabbable
{
    public Transform targetObject;
    public Vector3 localStartPos;
    public Quaternion localStartRot;

    private float maxAngle = 35f;
    private float minAngle = -35f;

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
            var newRot = Quaternion.LookRotation(m_grabbedBy.transform.position - transform.position, Vector3.up);
            targetObject.transform.rotation = Quaternion.Euler(0f, 0f, newRot.eulerAngles.z);
        }
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        isHeld = true;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);  

        isHeld = false;

        transform.SetParent(targetObject);
        transform.localPosition = localStartPos;
        transform.localRotation = localStartRot;
    }
}
