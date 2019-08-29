using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    static HapticsManager instance = null; // Lazy singleton 

    void Start()
    {
        if (HapticsManager.GetInstance() == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(this);
    }

    public static HapticsManager GetInstance()
    {
        return instance;
    }

    public static void Vibrate(AudioClip vibrationAudio, OVRInput.Controller controller)
    {
        OVRHapticsClip clip = new OVRHapticsClip(vibrationAudio);

        if (controller == OVRInput.Controller.LTouch)
        {
            OVRHaptics.LeftChannel.Preempt(clip);
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }

}
