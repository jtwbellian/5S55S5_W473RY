using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Fish : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidbod;
    private float speed = 1.2f;
    private FXManager fx;
    [ReadOnly]
    public bool canSwim = true;
    private SoundManager sm;
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {  
        rigidbod = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        speed = Random.Range(1.2f, 3f);

        rigidbod.freezeRotation = true;
        rigidbod.useGravity = true;
        fx = FXManager.GetInstance();
        sm = SoundManager.instance;

        if (!fx)
        {
            Debug.Log("FX Manager not found");
        }
    }

    void Update()
    {
        if (canSwim)
        {
            //rigidbod.Translate(Vector3.forward * -speed * Time.deltaTime, Space.World);
            transform.Translate(Vector3.forward * -speed * Time.deltaTime, Space.World);

            if (transform.parent != null)
                canSwim = false;
        }
        else if (animator.GetBool("Flop") == false)
        {
            animator.SetBool("Flop", true);
        }
    }

    //Splash into Water
    private void OnTriggerEnter(Collider other) 
    {
        // Reach end
        if ((other.transform.CompareTag("Finish") || other.transform.CompareTag("Right") || other.transform.CompareTag("Left")) && transform.parent == null)
        {
            var pa = GetComponent<PhotonActor>();

            if (pa)
            {
                //rigidbod.useGravity = true;
                pa.DisableChildObject(false);
            }
        }

        if (other.gameObject.tag=="Water" && transform.parent == null)
            {
                fx.Burst(FXManager.FX.Splash, transform.position, 5);
                fx.Burst(FXManager.FX.Ripple, transform.position -  Vector3.up * 0.3f, 1);
                fx.Burst(FXManager.FX.Spray, transform.position, 2);

                int randomIndex = Random.Range(0, clips.Length);
                sm.PlaySingle(clips[randomIndex], transform.position);

                canSwim = true;
                //rigidbod.freezeRotation = true;
                transform.rotation = Quaternion.identity;

            if (animator.GetBool("Flop") == true)
                {
                    rigidbod.velocity =  Vector3.zero;//new Vector3(0f, 0f, 0f);
                    rigidbod.angularVelocity = Vector3.zero;// new Vector3(0f, 0f, 0f);
                    transform.rotation = Quaternion.identity;
                    rigidbod.useGravity = false;
                    //rigidbod.freezeRotation = false;
                    animator.SetBool("Flop", false);
                }
            }
    }

/*sink and reorient
    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.tag=="Water")
        {
            if (gameObject.transform.position.y < 0 && rigidbod.useGravity == true)
                {
                rigidbod.velocity =  Vector3.zero;//new Vector3(0f, 0f, 0f);
                rigidbod.angularVelocity =Vector3.zero;// new Vector3(0f, 0f, 0f);
                transform.rotation= Quaternion.identity;
                rigidbod.useGravity = false;
                rigidbod.freezeRotation = true;
                }
        }
    }*/

/* 
   private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.tag=="Water" && transform.parent == null)
        {
            rigidbod.useGravity = true;
            rigidbod.freezeRotation = true;
            animator.SetBool("Flop", true);  
            canSwim = false;
        } 
    }
*/
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
