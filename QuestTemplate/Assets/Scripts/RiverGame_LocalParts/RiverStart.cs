using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The difference between RiverSegment and RiverStart 
/// is that RiverStart components are in the room BEFORE 
/// the photon synchronized parts are spawned. 
/// This is left over from an earlier version where this
/// actually mattered. 
/// </summary>
public class RiverStart : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {

        // If reaches end trigger, set inactive and add the next segment
        if (other.CompareTag("Finish"))
        {
            gameObject.SetActive(false);
            RiverManager.instance.AddSeg();
        }
    }
}
