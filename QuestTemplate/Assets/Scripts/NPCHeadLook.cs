using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHeadLook : MonoBehaviour
{
    public GameObject target;
    private Animator myAnimator;
    private bool lineOfSight;
    private float targetNoticeDistance;
    private float targetRealDistance; 

    void Start()
    {
        SetInitialReferences(); 
    }

    void SetInitialReferences()
    {
        myAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            if (target != null)
            {
                targetNoticeDistance = Vector3.Distance(other.transform.position, transform.position);
            }
            if (lineOfSight == false)
                lineOfSight = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            if (lineOfSight == true)
                lineOfSight = false;
        }
    }

    void OnAnimatorIK()
    {
        if (myAnimator.enabled)
        {
            if (lineOfSight == true && target!=null)
            {
                targetRealDistance = Vector3.Distance(target.transform.position, transform.position);
                myAnimator.SetLookAtWeight(1f, 0, 1f - (targetRealDistance / targetNoticeDistance)/1.4f, 0, 0.7f);
                myAnimator.SetLookAtPosition(target.transform.position);
                Debug.Log((targetRealDistance/targetNoticeDistance).ToString());
            }
            else
            {
                myAnimator.SetLookAtWeight(0);
            }
        }
    }
}
