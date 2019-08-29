using UnityEngine;

public class TriggerAnimate : MonoBehaviour
{
    public bool persistAfterDeparture;
    public bool invertActivation;
    public string animgraphBoolName;
    private Animator animator;

    private bool isPlaying = false;

    public VRButton vrButton;
    private AudioClip vibrationClip;

    public AudioClip[] clips;
    
    void Start()
    {
        animator = GetComponent<Animator>();

        if (vrButton == null)
            vrButton = GetComponent<VRButton>();
        //vibrationClip = vrButton.vibrationClip;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPlaying)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            OVRGrabber hand = other.GetComponent<OVRGrabber>();
            Debug.Log("hand: " + hand);
            
            if (clips[0] != null)
            SoundManager.instance.PlaySingle(clips[0], transform.position);
            
            if (hand && vrButton.vibrationClip)
            {
                HapticsManager.Vibrate(vrButton.vibrationClip, hand.m_controller);
            }

            if (invertActivation == false)
                animator.SetBool(animgraphBoolName, true);

            if (invertActivation == true)
                animator.SetBool(animgraphBoolName, false);

            Invoke("Disable", 1f);
            isPlaying = true;
        }
    }

    void Disable()
    {
        if (vrButton != null)
            vrButton.Click();
            
        transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider Other)
    {
        if (Other.gameObject.transform.root.tag == "Player")
        {
            if (persistAfterDeparture == false)

                if (invertActivation == false)
                    animator.SetBool(animgraphBoolName, false);

                if (invertActivation == true)
                    animator.SetBool(animgraphBoolName, true);
        }
    }
}