using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y>0.215)
            animator.SetBool("Flop", true);
        else
        {
            animator.SetBool("Flop", false);
        }
    }
}
