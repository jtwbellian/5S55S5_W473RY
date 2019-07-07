using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;
    public int currentSpawn = 0;

    public Transform [] spawnPoints; 

    void Awake() 
    {
        GS = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update(){}

    public void OnPlayerSpawned()
    {
        if (currentSpawn < spawnPoints.Length)
        {
            currentSpawn ++;
        }
        else
        {
            currentSpawn = 0;
        }
    }
}
