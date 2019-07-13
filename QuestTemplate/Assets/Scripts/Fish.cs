using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidbod;

    // Start is called before the first frame update
    void Start()
    {  
        rigidbod = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
//Splash into Water
    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.root.gameObject.layer==4)
            {
                FXManager.GetInstance().Burst(FXManager.FX.Splash, transform.position, 5);
                FXManager.GetInstance().Burst(FXManager.FX.Ripple, transform.position, 1);
                FXManager.GetInstance().Burst(FXManager.FX.Spray, transform.position, 2);
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
                rigidbod.velocity = new Vector3(0f, 0f, 0f);
                rigidbod.angularVelocity = new Vector3(0f, 0f, 0f);
                transform.rotation=Quaternion.identity;
                rigidbod.useGravity = false;
                }
        }
    }

   private void OnTriggerExit(Collider other) 
    {
        if (other.transform.root.gameObject.layer==4)
            {
            rigidbod.useGravity = true;
            animator.SetBool("Flop", true);  
            } 
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
