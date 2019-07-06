using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public GameObject target;
    public bool persistAfterDeparture;
    public bool invertActivation;

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.transform.root.tag == "Player")
        {
            if (target != null)
            {
                if (invertActivation==false && target.activeSelf==false)
                target.gameObject.SetActive(true);
                if (invertActivation==true && target.activeSelf==true)
                target.gameObject.SetActive(false);
            }

        }
    }
    private void OnTriggerExit(Collider Other)
    {
        if (Other.gameObject.transform.root.tag == "Player")
        {
            if (persistAfterDeparture==false)
            if (target != null)
            { 
                if (invertActivation==false && target.activeSelf==true)
                    target.gameObject.SetActive(false);
                if (invertActivation==true && target.activeSelf==false)
                    target.gameObject.SetActive(true);
            }
        }
    }
}