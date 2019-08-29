using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnImpact : MonoBehaviour
{
    public AudioClip[] clips;
    private SoundManager sm;
    

    // Start is called before the first frame update
    void Start()
    {
        sm = SoundManager.instance;
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Boat")
        {

            int randomIndex = Random.Range(0, clips.Length);
            if (sm)
                sm.PlaySingle(clips[randomIndex], transform.position);
        
        }
    }
}
