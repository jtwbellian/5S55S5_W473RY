using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SharedSpace
{
    public static bool hasBeenCalibrated = false;
    public static float yOffset;
    public static System.Action onCalibrated;


    /* 
        Orients the given playerTrackingSpace to be an increment of 90 

    */
    public static void CalibrateByRotation(Transform playerTrackingSpace, Transform playerHeadTracker)
    {
        Debug.Log("Calibrating based on current head look direction ");

        float yOffset = (int)(playerHeadTracker.rotation.eulerAngles.y / 90) * 90f;

        playerTrackingSpace.transform.rotation  = Quaternion.Euler(0f, yOffset, 0f);
        SharedSpace.yOffset = yOffset;

        if (onCalibrated != null)
            onCalibrated.Invoke();

        SharedSpace.hasBeenCalibrated = true;
    }

    public static void CalibrateByValue(Transform playerTrackingSpace, float yOffset)
    {
        Debug.Log("Calibrating based on previous orientation");
        playerTrackingSpace.transform.rotation  = Quaternion.Euler(0f, yOffset, 0f);

        if (onCalibrated != null)
            onCalibrated.Invoke();

        SharedSpace.hasBeenCalibrated = true;
    }

    public static void SaveForwardOffset()
    {
        PlayerPrefs.SetInt("PlaySpaceForward", (int) yOffset);
    }

    public static int LoadForwardOffset()
    {
        float yRotation =  (float) PlayerPrefs.GetInt("PlaySpaceForward", -1);

        if (yRotation == -1)
        {
            Debug.Log("No Calibration previously stored");
            return (int) yRotation;
        }

        Debug.Log("Loaded offset of " + yRotation);

        return (int) yRotation;
    }

    public static void ClearPreferences()
    {
        PlayerPrefs.SetInt("PlaySpaceForward", -1);
    }

}
