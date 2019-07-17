using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RiverSegment : MonoBehaviour
{
    private GameManager gm;
    private Coin [] coins;
    public Transform endPoint;
    public PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        coins = GetComponentsInChildren<Coin>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        var newPos = transform.position + Vector3.forward * gm.levelSpeed * Time.deltaTime;
        transform.position = newPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            foreach (var c in coins)
            {
                c.gameObject.SetActive(true);
            }

            pv.RPC("Activate", RpcTarget.All, false);
            gm.AddRiver();
        }
    }

    [PunRPC]
    void Activate(bool on)
    {
        gameObject.SetActive(on);
    }
}
