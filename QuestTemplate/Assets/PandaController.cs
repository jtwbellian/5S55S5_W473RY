using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaController : OVRPlayerController
{
    public Transform trackingPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("Pressing X");
            transform.position = GameManager.instance.boat.position - trackingPoint.localPosition + (Vector3.up * 3f);
        }
    }
}
