using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("load");
    }

    public string levelName;
    AsyncOperation async;

    IEnumerator load()
    {
        Debug.LogWarning("ASYNC LOAD STARTED - " +
           "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
        async = SceneManager.LoadSceneAsync(levelName);
        //async.allowSceneActivation = false;
        yield return async;
    }

    public void ActivateScene()
    {
        async.allowSceneActivation = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
