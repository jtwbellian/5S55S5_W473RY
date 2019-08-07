using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    RiverManager rm;
    private FXManager fx;

    // Start is called before the first frame update
    void Start()
    {
        fx = FXManager.GetInstance();

        if (!fx)
        {
            Debug.Log("FX Manager not found");
        }

        rm = RiverManager.instance;
        
        if (!rm)
            Debug.Log("River Manager not found.");
    }

private void OnTriggerEnter(Collider other) 
    {

        // Reach end
        if (other.transform.CompareTag("Finish"))
        {
            var pa = GetComponent<PhotonActor>();

            if (pa)
            {
                pa.DisableChildObject(false);
            }
        }

        if (other.gameObject.tag=="Boat")
        {
            fx.Burst(FXManager.FX.LogSplit, transform.position, 4);
            fx.Burst(FXManager.FX.Mist, transform.position + Vector3.forward * 1.2f, 2);
            fx.Burst(FXManager.FX.Mist, transform.position + Vector3.forward * -1.2f, 2);
            fx.Burst(FXManager.FX.Spray, transform.position, 2);
            fx.Burst(FXManager.FX.Ripple, transform.position, 1);
        }
    }

    void OnTriggerStay(Collider other) 
    {

        if (other.transform.CompareTag("Left"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 3f, Space.World);
        }

        if (other.transform.CompareTag("Right"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * -3f, Space.World);
        }
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