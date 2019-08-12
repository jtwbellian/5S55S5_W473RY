using UnityEngine;

public class TriggerAnimate : MonoBehaviour
{
    public bool persistAfterDeparture;
    public bool invertActivation;
    public string animgraphBoolName;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.transform.root.tag == "Player")
        {
            if (invertActivation == false)
                animator.SetBool(animgraphBoolName, true);

            if (invertActivation == true)
                animator.SetBool(animgraphBoolName, false);

            Invoke("Disable", 1.3f);
        }
    }

    void Disable()
    {
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