using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void OnTriggerEnter(Collider other) {
        var rs = other.GetComponent<RiverSegment>();

       if (rs != null) 
       {
           GameManager.instance.rotationOffset += rs.myAngle;
           GameManager.instance.targetSegment = rs;
       }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
