using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Transform headTarget = null;
    public Transform lhandTarget = null;
    public Transform rhandTarget = null;

    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (head != null)
        {
            head.transform.position = headTarget.position;
            head.transform.rotation = headTarget.rotation;
        }
    }
}
