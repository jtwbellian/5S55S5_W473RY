using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaController : MonoBehaviour//OVRPlayerController
{
    public const float MAX_DIST_FROM_BOAT = 5.5f;
    public Transform trackingPoint;
    public float speed = 0.4f;
    
    private OVRScreenFade fade;
    private CharacterController character;

    #region viewControl
    private bool ReadyToSnapTurn = false;
    private Vector3 euler;
    private Vector3 lastPos; 
    public RiverManager riverManager;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        riverManager = RiverManager.instance;
        fade = GetComponentInChildren<OVRScreenFade>();
        //Invoke("GoHome", 0.25f);
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        character.enabled = false;
        character.height = trackingPoint.localPosition.y;
        character.center = trackingPoint.transform.localPosition - (Vector3.up * character.height) / 2f;
        character.enabled = true;

        if (riverManager != null)
            if (OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.Four) || 
                Vector3.Distance(transform.position, riverManager.boat.transform.position) > MAX_DIST_FROM_BOAT)
            {
                Debug.Log("Teleport Home");
                GoHome();
            }
        
        // Walk
        Vector2 stickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        character.SimpleMove((trackingPoint.forward  * stickInput.y * speed) + (trackingPoint.right * stickInput.x * speed));

        // Oculus Go support 
        Vector2 touchInput = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        character.SimpleMove((trackingPoint.forward * touchInput.y * speed) + (trackingPoint.right * touchInput.x * speed));


        // Turn View Left
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
        {
            if (ReadyToSnapTurn)
            {
                ViewRatchet(-45f);
            }
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
        {
            if (ReadyToSnapTurn)
            {
                ViewRatchet(45f);
            }
        }
        else if (ReadyToSnapTurn == false)
        {
            ReadyToSnapTurn = true;
        }
    }

    public void ViewRatchet(float amt)
    {
        euler = transform.rotation.eulerAngles;
        lastPos = trackingPoint.position;

        euler.y += amt;
        transform.rotation = Quaternion.Euler(euler);
        transform.position += lastPos - trackingPoint.position;
        ReadyToSnapTurn = false;
    }


    public void GoHome()
    {
        if (riverManager == null)
            return;
        fade.FadeIn();
        //character.center = trackingPoint.transform.position;
        character.enabled = false;
        transform.position = riverManager.boat.transform.position + Vector3.up * 0.7f;//(RiverManager.instance.boat.transform.position - trackingPoint.position);// + trackingPoint.localPosition);// + (Vector3.up * 3f);
        character.enabled = true;
    }
}
