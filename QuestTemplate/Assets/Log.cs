using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    RiverManager rm;
    // Start is called before the first frame update
    void Start()
    {
        rm = RiverManager.instance;
        
        if (!rm)
            Debug.Log("River Manager not found.");
    }

    // Update is called once per frame
    void Update()
    {
        if (rm)
        {
            transform.Translate(rm.riverVelocity + rm.boatRightVelocity, Space.World);
        }
    }
}
