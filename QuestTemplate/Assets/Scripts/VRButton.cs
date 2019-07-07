using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public UnityEvent OnClick;

    // Start is called before the first frame update
    void Start()
    {}

    private void OnTriggerEnter(Collider other) {
        OnClick.Invoke();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
