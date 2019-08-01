using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RiverSegment : MonoBehaviour
{
    private RiverManager rm;
    private Coin [] coins;
    public Transform endPoint;
    public PhotonView pv;
    public GameObject [] fruits;

    public float myAngle;

    // Start is called before the first frame update
    void Start()
    {
        rm = RiverManager.instance;
        //coins = GetComponentsInChildren<Coin>();
        pv = GetComponent<PhotonView>();
        //pv.RPC("Activate", RpcTarget.All, false);
    }

    // Update is called once per frame
    void Update()
    {
        /* var newPos = transform.position + Vector3.forward * rm.levelSpeed * Time.deltaTime;
        transform.position = newPos;*/
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            rm.activeParts--;
            DisableChildObject(false);
            rm.AddRiver();
        }
    }

 [PunRPC]
 void RemoveBlock(int BlockToRemove, bool setActive)
 {
     PhotonView Disable = PhotonView.Find(BlockToRemove);
     Disable.transform.gameObject.SetActive(setActive);
 }

  public void DisableChildObject(bool setActive)
 {
         GetComponent<PhotonView>().RPC("RemoveBlock", RpcTarget.AllBuffered, transform.gameObject.GetComponent<PhotonView>().ViewID, setActive);
 }
 /* 

    [PunRPC]
    void Activate(bool on)
    {
        gameObject.SetActive(on);
        Debug.Log( gameObject.name + ": Active = " + on.ToString());
        
    }*/
}
