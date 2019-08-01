using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaController : OVRPlayerController
{
    public Transform trackingPoint;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        //Invoke("GoHome", 1f);
        //character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            Debug.Log("Pressing X");
            GoHome();
        }

        //character.center = trackingPoint.transform.localPosition;
    }

    public void GoHome()
    {
        transform.position = (RiverManager.instance.boat.transform.position - trackingPoint.localPosition);// + trackingPoint.localPosition);// + (Vector3.up * 3f);
    }
}
