using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StaticBeforeHit : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidbod;
    // Start is called before the first frame update
    /* void Start()
    {
        rigidbod = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rigidbod.useGravity = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!rigidbod.useGravity)
        {
            transform.SetParent(null);
            rigidbod.useGravity = true;
            rigidbod.isKinematic = false;
        }
    }
    // Update is called once per frame
    private void OnCollisionEnter(Collision other) 
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!rigidbod.useGravity)
        {
            transform.SetParent(null);
            rigidbod.useGravity = true;
            rigidbod.isKinematic = false;
        }
    }*/
}
