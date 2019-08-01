using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    //GameManager gm;
    float spinSpeed = 8f;

    // Start is called before the first frame update
    void Start()
    {
        //gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        var newRot = transform.rotation.eulerAngles + new Vector3(0, 5f, 0);
        transform.rotation = Quaternion.Euler(newRot);
    }
}
