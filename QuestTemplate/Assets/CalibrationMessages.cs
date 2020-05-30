using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationMessages : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SharedSpace.onCalibrated += Disable;
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
