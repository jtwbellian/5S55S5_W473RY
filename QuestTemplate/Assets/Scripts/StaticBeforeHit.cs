using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBeforeHit : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidbod;
    // Start is called before the first frame update
    void Start()
    {
    rigidbod = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();
    rigidbod.useGravity=false;
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other) 
    {
        if (rigidbod.useGravity==false)
            rigidbod.useGravity=true;
    }
}
