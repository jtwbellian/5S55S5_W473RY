using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVRGrabber : OVRGrabber
{

    protected override void OffhandGrabbed(OVRGrabbable grabbable)
    {
        base.OffhandGrabbed(grabbable);
    }

    protected override void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
		OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
		foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        // Added to prevent grabbing an object held by another user
        POVRGrabbable photonGrabbable = closestGrabbable as POVRGrabbable; 
        var isHeld = false;

        if (photonGrabbable)
        {
            if (photonGrabbable.isHeld)
            {
                // If is held by me, offhand grab
                if (photonGrabbable.transform.root == transform.root)
                {                
                    (photonGrabbable.grabbedBy as POVRGrabber).OffhandGrabbed(closestGrabbable);
                }
                else // otherwise is held by another user
                {
                    isHeld = true;
                }
            }
        }

        //Debug.Log("Grabbing object " + photonGrabbable + ", isheld = " + isHeld.ToString());

        if (isHeld)
            return;
        ////////////////////////////////////////////////////////////
        
        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                (closestGrabbable.grabbedBy as POVRGrabber).OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if(m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if(m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if(m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            if(m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }
}
