using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCatcher : MonoBehaviour
{
    public Transform target;
    public Transform rimBound;
    public Transform netBound;
    private float linearLimit = 0.02f;
    
    private void OnTriggerStay(Collider other) 
    {   //Instead of comparing rotation with Quaternions, the net is checking a linear ratio between two tranforms located at the net end and the rim end. 
        //By comparing the difference in y postions, the code can determine if the net is in a "dipping up" or "dipping down" position 
        //regardless of axis of rotation and grab the collectable accordingly.
        if (rimBound.position.y - netBound.position.y > linearLimit && other.gameObject.tag == "Collectable")
            {
            other.gameObject.transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
            }
    }
}