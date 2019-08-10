using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    RiverManager rm;
    private FXManager fx;
    private SoundManager sm;
    public AudioClip[] clips;
    private PhotonActor pa;

    // Start is called before the first frame update
    void Start()
    {
        pa = GetComponent<PhotonActor>();
        fx = FXManager.GetInstance();
        sm = SoundManager.instance;

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

        if (!rm.isHost)
            return; 

        // Reach end
        if (other.transform.CompareTag("Finish"))
        {
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

            //Play Random Sound From list
            int randomIndex = Random.Range(0, clips.Length);
            sm.PlaySingle(clips[randomIndex], transform.position);
            pa.DisableChildObject(false);
        }
    }

    void OnTriggerStay(Collider other) 
    {
        if (!rm.isHost)
            return; 

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
        if (rm.isHost)
        {
            // delete after 20m behind boat
            var behindBoatZ = (rm.boat.transform.position.z - 20f);
            
            if (transform.position.z < behindBoatZ)
            {
                pa.DisableChildObject(false);
            }

            transform.Translate(rm.riverVelocity + rm.boatRightVelocity, Space.World);
        }
    }
}