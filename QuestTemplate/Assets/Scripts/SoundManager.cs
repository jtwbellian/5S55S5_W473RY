using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource globalSound;
    //public AudioSource efxSource;
    //public AudioSource environmentSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    // Start is called before the first frame update
    void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy (gameObject);

        DontDestroyOnLoad (gameObject);
    }

    public void PlaySingle (AudioClip clip,  Vector3 pos) 
    {
        AudioSource.PlayClipAtPoint(clip, pos);
    }

    public void RandomizeSFX (AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        globalSound.pitch = randomPitch;
        globalSound.clip = clip;
        globalSound.Play();
    }
}
