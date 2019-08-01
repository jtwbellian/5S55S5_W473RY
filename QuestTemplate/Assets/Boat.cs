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
    void Start(){}
    
    void OnTriggerEnter(Collider other) 
    {

        if (other.transform.CompareTag("Left"))
        {
            rudder -= 2f;
        }

        if (other.transform.CompareTag("Right"))
        {
            rudder += 2f;
        }

        var rs = other.GetComponent<RiverSegment>();

       if (rs != null) 
       {
           RiverManager.instance.rotationOffset += rs.myAngle;
           RiverManager.instance.targetSegment = rs;
       }

    }

    void Update() 
    {

        rudder = rudder * 0.95f;
    }
}
