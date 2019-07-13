using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverSegment : MonoBehaviour
{
    private GameManager gm;
    private Coin [] coins;
    public Transform endPoint;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        coins = GetComponentsInChildren<Coin>();
    }

    // Update is called once per frame
    void Update()
    {
        var newPos = transform.position + Vector3.forward * gm.levelSpeed * Time.deltaTime;
        transform.position = newPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            foreach (var c in coins)
            {
                c.gameObject.SetActive(true);
            }

            gameObject.SetActive(false);
            gm.AddRiver();
        }
    }
}
