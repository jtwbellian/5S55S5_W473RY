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
        Invoke("GoHome", 1f);
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.One) )
        {
            Debug.Log("Teleport Home");
            GoHome();
        }
    }

    public void GoHome()
    {
        //character.center = trackingPoint.transform.position;
        character.enabled = false;
        transform.position = RiverManager.instance.boat.transform.position + Vector3.up * 1.5f;//(RiverManager.instance.boat.transform.position - trackingPoint.position);// + trackingPoint.localPosition);// + (Vector3.up * 3f);
        character.enabled = true;
    }
}
