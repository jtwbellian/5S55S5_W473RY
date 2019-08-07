using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
void OnTriggerEnter(Collider other) {

    if (other.CompareTag("Finish"))
    {
        gameObject.SetActive(false);
        //RiverManager.instance.AddRiver();
        //rm.RemoveSeg(id);
        RiverManager.instance.AddSeg();
    }
}
    // Update is called once per frame
    void Update()
    {
        
    }
}
