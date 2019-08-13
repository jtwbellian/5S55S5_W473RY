using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public UnityEvent OnClick;
    private bool active = false;
    public AudioClip vibrationClip;

    // Start is called before the first frame update
    void Start()
    {
    }

    [ContextMenu("Click")]
    public void Click()
    {
        OnClick.Invoke();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OVRGrabber hand = other.GetComponent<OVRGrabber>();

            if (hand && vibrationClip)
            {
                HapticsManager.Vibrate(vibrationClip, hand.m_controller);
            }

            OnClick.Invoke();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
