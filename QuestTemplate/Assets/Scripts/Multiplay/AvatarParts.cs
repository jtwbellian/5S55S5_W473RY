using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class AvatarParts : MonoBehaviour
{
    public Transform Head, RHand, LHand;
    public GameObject scarfRed, scarfBlue;
    public Renderer headRenderer;

    /* public void SetScarf(string scarf)
    {
        //Head.GetComponent<PhotonView>().RPC("RPC_SetScarf", RpcTarget.AllBuffered, scarf);
    }*/

   /*  [PunRPC]
    public void RPC_SetScarf(string scarf)
    {
        Debug.Log("Setting Scarf");

        scarf = scarf.ToLower();

        if (scarf == "red")
        {
            scarfBlue.SetActive(false);
            return;
        }

        if (scarf == "blue")
        {
            scarfRed.SetActive(false);
            return;
        }
    }*/
}
