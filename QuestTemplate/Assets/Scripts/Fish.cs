using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    Animator animator;
    Rigidbody rigidbod;

    // Start is called before the first frame update
    void Start()
    {  
        rigidbod = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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
    }
}
