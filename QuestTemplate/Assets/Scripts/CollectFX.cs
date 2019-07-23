using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectFX : MonoBehaviour
{
    public Transform target;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Collectable")
            {
            FXManager.GetInstance().Burst(FXManager.FX.Confetti1, target.position, 1);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti2, target.position, 15); 
            Destroy(other.gameObject);
            }
               
    }
}
