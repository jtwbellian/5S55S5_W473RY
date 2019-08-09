using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressMeter : MonoBehaviour
{
    public float percentComplete;

    RiverManager rm;
    [SerializeField]
    private TextMeshPro label;

    // Start is called before the first frame update
    void Start()
    {
        rm = RiverManager.instance;
        InvokeRepeating("UpdateValues", 30f, 1f);
    }


    public void UpdateValues()
    {
        percentComplete = rm.GetPercentComplete();
        
        if (this.transform.localPosition.x != percentComplete * 0.008f - 0.4f)
            this.transform.localPosition = new Vector3(percentComplete * 0.008f - 0.4f, transform.localPosition.y, transform.localPosition.z);

        label.text = ((int)percentComplete).ToString() + "%";

        Debug.Log("calculated percent complete");
    }
}
