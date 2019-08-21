using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private const float TOL = 2f;
    public Vector3 positionOffset = Vector3.zero;
    public float speed = 3.5f;
    public AudioClip clip;

    [Range(-10, 10)]
    public float rudder = 0f;
    private RiverManager riverManager;

    // Start is called before the first frame update
    void Start()
    {
        riverManager = RiverManager.instance;
    }
    
    void OnTriggerStay(Collider other) 
    {
        if (!RiverManager.instance.isHost) 
        {
            return;
        }

        if (other.transform.CompareTag("Left"))
        {
            if (rudder > TOL)
                rudder = 0;

            rudder -= speed * Time.deltaTime;
        }

        if (other.transform.CompareTag("Right"))
        {
            if (rudder < -TOL)
                rudder = 0;

            rudder += speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other) 
    {

        if (other.transform.CompareTag("FinishLine"))
        {
            riverManager.levelSpeed = 0f;
            riverManager.GameOver();
        }

        if (other.transform.CompareTag("Left") || other.transform.CompareTag("Right"))
        {
            SoundManager.instance.RandomizeSFX(clip);
        }

        if (!riverManager.isHost) 
        {
            return;
        }
        
        var rs = other.GetComponent<RiverSegment>();

       if (rs != null)
       {
           riverManager.rotationOffset += rs.myAngle;
           riverManager.targetSegment = rs;
       }
    }

    void Update() 
    {
        if (!riverManager.isHost) 
        {
            return;
        }
        
        rudder = rudder * 0.95f;
    }
}
