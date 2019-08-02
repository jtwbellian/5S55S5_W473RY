using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressMeter : MonoBehaviour
{
    public float percentComplete;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    if (this.transform.localPosition.x != percentComplete*0.008f - 0.4f)
        this.transform.localPosition = new Vector3(percentComplete*0.008f - 0.4f, transform.localPosition.y, transform.localPosition.z);
    }
}
