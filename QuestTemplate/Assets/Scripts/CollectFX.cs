using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectFX : MonoBehaviour
{
    public Transform target;
    public AudioClip clip;
    private RiverManager rm;
    private SoundManager sm;

    private void Start() 
    {
        rm = RiverManager.instance;
        sm = SoundManager.instance;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Collectable")
        {
           var rm = RiverManager.instance;

            sm.RandomizeSFX(clip);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti1, target.position, 1);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti2, target.position, 15); 

            if (rm.isHost)
            {
                var pa = other.gameObject.GetComponent<PhotonActor>();

                if (pa.transform.parent != null)
                {
                    var net = pa.transform.root.GetComponentInChildren<NetCatcher>();

                    if (net)
                    {
                        net.caughtItem = null;
                    }
                    else 
                    Debug.Log("ERR couldnt find net");

                    pa.transform.SetParent(null);
                }

                
                if (pa)
                    pa.DisableChildObject(false);

                /////////////////////////////////////////////////////////////////////////////////////////////////

                // Now we also want to increase the points by accessing the RiverManager, which is the static object that controls most of the game.
                // He is going to pass a message to his parallel universe RiverManager counterpart so that they can both call the function. 

                var item = other.transform.root.GetComponent<Item>();

                if (rm && item) // We do this to avoid the Null error. Now the game will just not work right instead of crashing / throwing an error. If we want we can throw our own debug.log error
                {
                    // This needs to be replaced by code that checks who put the fish in the barrel, and also what kind of item it was
                    rm.AddPoints(item.owner, item.type, 1);
                }
                else
                {
                    Debug.Log("Error, either the collectible was missing Item Componenet or the River Manager could not be found.");
                }

                item.owner = -1;
            }

            //Destroy(other.gameObject); 

            // This part does the same thing, except it is disabled across the Photon Servers and not deleted. 
            // This way we can use the same fish again later with less overhead.
        }     
    }
}
