using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FruitTree : MonoBehaviour
{
    public AudioClip clip;
    RiverManager rm;
    SoundManager sm;
    FXManager fx;
    PhotonActor pa;

    [SerializeField]
    PhotonFruit [] apples;
    Vector3 [] startPositions;

    public bool isClamHole = false;

    // Start is called before the first frame update
    void Awake()
    {
        rm = RiverManager.instance;
        fx = FXManager.GetInstance();
        sm = SoundManager.instance;
        pa = GetComponent<PhotonActor>();
        //Debug.Log("Apples on tree: " + apples.Length + ", " + apples);
        startPositions = new Vector3[apples.Length];

        for ( var i = 0; i < apples.Length - 1; i ++)
        {
            startPositions[i] = apples[i].gameObject.transform.localPosition;
        }
    }

    private void OnEnable() 
    {
        ChildToRiver();
    }

    [PunRPC] 
    public void RPC_ChildToRiver()
    {
        /* if (!active)
        {
            transform.SetParent(null);
            return;
        }*/
        if (rm)
        {
            transform.SetParent(rm.riverMaster);
            //Debug.Log("Childed to River");
        }
        
        var i = 0;

        if (startPositions.Length == apples.Length)
            foreach (var apple in apples)
            {
                if (apple.gameObject.activeInHierarchy)
                    continue;

                apple.actor.DisableChildObject(true);
                apple.ChildToPhotonTransform(transform, startPositions[i], Quaternion.identity * Quaternion.Euler(90, 0, 0));
                Debug.Log("Adding element " + apple + " to clamHole/fruitTree");
                i ++;
            }
    }

    [PunRPC]
    public void RPC_Destroy()
    {
        if (!isClamHole)
        {        
            fx.Burst(FXManager.FX.TreeSplit, transform.position, 2);
            fx.Burst(FXManager.FX.TreeSplit, transform.position + Vector3.up * 1.8f, 2);
            fx.Burst(FXManager.FX.Mist, transform.position + Vector3.forward * 1.2f, 2);
            fx.Burst(FXManager.FX.Mist, transform.position + Vector3.forward * -1.2f, 2);
            fx.Burst(FXManager.FX.Dust, transform.position + Vector3.forward * 1.1f, 2);
            fx.Burst(FXManager.FX.Dust, transform.position + Vector3.forward * -1.1f, 2);
            fx.Burst(FXManager.FX.Spray, transform.position, 2);
            fx.Burst(FXManager.FX.Ripple, transform.position, 1);

            //Play  Sound
            sm.PlaySingle(clip, transform.position);

            foreach (var apple in apples)
            {
                apple.ActivateFruit();
            }
        }
    }

    public void ChildToRiver()
    {
        GetComponent<PhotonView>().RPC("RPC_ChildToRiver", RpcTarget.AllBuffered);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.CompareTag("Boat"))
        {
            pa.DisableChildObject(false);
            pa.view.RPC("RPC_Destroy", RpcTarget.AllBuffered);
        }

        if (other.transform.CompareTag("Finish"))
        {
            pa.DisableChildObject(false);
        }
    }

    /*public void SetBumperDirection(string tag)
    {
        bumper.tag = tag;
    }*/

    private void Update() 
    {
        // delete after 20m behind boat
        var behindBoatZ = (rm.boat.transform.position.z - 20f);
        
        if (transform.position.z < behindBoatZ)
        {
            pa.DisableChildObject(false);
        }
    }
}
