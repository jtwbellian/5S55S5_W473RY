using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public GameObject target;
    public bool persistAfterDeparture;

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.transform.root.tag == "Player")
        {
            if (target != null && target.activeSelf==false)
            {
                target.gameObject.SetActive(true);
            }

        }
    }
    private void OnTriggerExit(Collider Other)
    {
        if (Other.gameObject.transform.root.tag == "Player")
        {
            if (target != null && target.activeSelf==true && persistAfterDeparture==false)
            { 
                    target.gameObject.SetActive(false);
            }
        }
    }
}