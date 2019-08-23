using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public UnityEvent OnClick;
    private bool active = false;
    public AudioClip vibrationClip;

    private bool hasBeenPressed = false;
    public bool touchToActivate = false;

    // Start is called before the first frame update
    void Start(){}

    [ContextMenu("Click")]
    public void Click()
    {
        OnClick.Invoke();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (hasBeenPressed)
            return;


        if (other.gameObject.CompareTag("Player") && touchToActivate)
        {
            OVRGrabber hand = other.GetComponent<OVRGrabber>();

            if (hand && vibrationClip)
            {
                HapticsManager.Vibrate(vibrationClip, hand.m_controller);
            }

            OnClick.Invoke();
            hasBeenPressed = true;
        }
    }
}
