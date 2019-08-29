using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverSegment : MonoBehaviour
{
    private RiverManager rm;
    public Transform endPoint; // Where the segment ends and the next should begin

    public float myAngle; // Angle in degrees that the endpoint turns and the river needs to compensate for

    public int id = -1;

    void Start()
    {
        rm = RiverManager.instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish") // disable and enable to the segment
        {
            rm.activeParts--;

            rm.AddSeg();
            gameObject.SetActive(false);
        }
    }
}
