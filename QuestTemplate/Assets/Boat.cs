using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private const float TOL = 0.05f;
    public Vector3 positionOffset = Vector3.zero;
    public float speed = 3f;

    [Range(-10, 10)]
    public float rudder = 0f;
    // Start is called before the first frame update
    void Start(){}
    
    void OnTriggerStay(Collider other) 
    {
        if (!RiverManager.instance.isHost) 
        {
            return;
        }

        if (other.transform.CompareTag("Left"))
        {
            rudder -= speed * Time.deltaTime;
        }

        if (other.transform.CompareTag("Right"))
        {
            rudder += speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (!RiverManager.instance.isHost) 
        {
            return;
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
        if (!RiverManager.instance.isHost) 
        {
            return;
        }
        
        rudder = rudder * 0.95f;
    }
}
