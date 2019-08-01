﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Fish : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidbod;
    private float speed = 2f;
    private FXManager fx;

    // Start is called before the first frame update
    void Start()
    {  
        rigidbod = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rigidbod.velocity = Vector3.forward * speed;
        rigidbod.freezeRotation = true;
        fx = FXManager.GetInstance();

        if (!fx)
        {
            Debug.Log("FX Manager not found");
        }
    }
//Splash into Water
    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.root.gameObject.layer==4)
            {
                fx.Burst(FXManager.FX.Splash, transform.position, 5);
                fx.Burst(FXManager.FX.Ripple, transform.position, 1);
                fx.Burst(FXManager.FX.Spray, transform.position, 2);

            if (animator.GetBool("Flop")==true)
                {
                    animator.SetBool("Flop", false);
                }
            }
    }

//sink and reorient
    private void OnTriggerStay(Collider other) 
    {
        if (other.transform.root.gameObject.layer==4)
        {
            if (gameObject.transform.position.y<0 & rigidbod.useGravity == true)
                {
                rigidbod.velocity = Vector3.zero;//new Vector3(0f, 0f, 0f);
                rigidbod.angularVelocity = Vector3.zero;//new Vector3(0f, 0f, 0f);
                transform.rotation= Quaternion.identity;
                rigidbod.useGravity = false;
                rigidbod.freezeRotation = true;
                }
        }
    }

   private void OnTriggerExit(Collider other) 
    {
        if (other.transform.root.gameObject.layer==4)
            {
            rigidbod.useGravity = true;
            rigidbod.freezeRotation = true;
            animator.SetBool("Flop", true);  
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
    /* Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y>0)
        {
            rigidbod.useGravity = true;
            animator.SetBool("Flop", true);
        }
        
        else if (rigidbod.useGravity == true)
        {
            rigidbod.useGravity = false;
            rigidbod.velocity = new Vector3(0, 0, 0);
            animator.SetBool("Flop", false);
        }
    } */
}
