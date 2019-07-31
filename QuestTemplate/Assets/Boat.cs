using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private const float TOL = 0.05f;
    public Vector3 positionOffset = Vector3.zero;

    [Range(-10, 10)]
    public float rudder = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void OnTriggerEnter(Collider other) 
    {
        var rs = other.GetComponent<RiverSegment>();

       if (rs != null) 
       {
           GameManager.instance.rotationOffset += rs.myAngle;
           GameManager.instance.targetSegment = rs;
       }

    }

    void Update() 
    {
    }
}
