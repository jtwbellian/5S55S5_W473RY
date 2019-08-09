using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiSpawn : MonoBehaviour
{
    public AudioClip clip;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Boat")
        {
        FXManager.GetInstance().Burst(FXManager.FX.Confetti2, transform.position + Vector3.right * 1.8f, 15); 
        FXManager.GetInstance().Burst(FXManager.FX.Confetti2, transform.position + Vector3.left * 1.8f, 15); 
        FXManager.GetInstance().Burst(FXManager.FX.Confetti2, transform.position, 15); 

        SoundManager.instance.PlaySingle(clip, transform.position);
        }
    }
}
