using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FruitTree : MonoBehaviour
{
    RiverManager rm;
    PhotonActor pa;

    [SerializeField]
    PhotonFruit [] apples;
    Vector3 [] startPositions;

    // Start is called before the first frame update
    void Awake()
    {
        rm = RiverManager.instance;
        pa = GetComponent<PhotonActor>();
        Debug.Log("Apples on tree: " + apples.Length + ", " + apples);
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
            Debug.Log("Childed to River");
        }
        
        var i = 0;
        if (startPositions.Length == apples.Length)
            foreach (var apple in apples)
            {
                if (apple.gameObject.activeInHierarchy)
                    continue;

                apple.actor.DisableChildObject(true);
                apple.ChildToPhotonTransform(transform, startPositions[i], Quaternion.identity * Quaternion.Euler(90, 0, 0));
                i ++;
            }
    }

    public void ChildToRiver()
    {
        GetComponent<PhotonView>().RPC("RPC_ChildToRiver", RpcTarget.AllBuffered);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.CompareTag("Finish"))
        {
            if (pa)
            {
                pa.DisableChildObject(false);
            }
        }
    }

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
