﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.IO;

public class RiverManager : MonoBehaviour
{
    private List<GameObject> RiverPool = new List<GameObject>();

    [HideInInspector]
    public float levelSpeed = 0f;

    public int score = 0;
    public int lives = 3;

    public int numSegments = 5;
    public bool gameOver = false;
    public string [] RiverTypes;
    public RiverSegment lastRiverSegment;

    #region singleton implementation
    public static RiverManager instance;   
    public Transform oarSpawnA, oarSpawnB;

    void Awake()
    {
        if (RiverManager.instance != this)
        {
            RiverManager.instance = this;
        }
    }
#endregion

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Am master, building level..");
            CreateRiverPool();
            PhotonNetwork.InstantiateSceneObject(Path.Combine("PhotonPrefabs", "Paddle"),
                                        oarSpawnA.position, 
                                        oarSpawnB.rotation, 0, null);
            PhotonNetwork.InstantiateSceneObject(Path.Combine("PhotonPrefabs", "Paddle"),
                                        oarSpawnB.position, 
                                        oarSpawnB.rotation, 0, null);
        }
    }
    
    // Initializes Our Object Pool of River segments, disabling all at first
    public void CreateRiverPool()
    {
        // create the River prefabs
        for (int i = 0; i < RiverTypes.Length; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                //GameObject block = Instantiate(RiverTypes[i]);

                GameObject block = PhotonNetwork.InstantiateSceneObject(Path.Combine("RiverSegments", RiverTypes[i]),
                                        Vector3.zero, 
                                        Quaternion.identity, 0, null);

                RiverPool.Add(block);
                block.SetActive(false);
            }
        }

        // Build first n River segments
        for (int i = 0; i < numSegments; i++)
        {
            AddRiver();
        }
    }

    public void AddRiver()
    {
        // Shuffle the pool to mix it up
        RandomizePool();

        // Finds the first inactive segment and lays it down
        for (int i = 0; i < RiverPool.Count - 1; i++)
        {
            if (!RiverPool[i].activeInHierarchy)
            {
                RiverPool[i].transform.SetPositionAndRotation(lastRiverSegment.endPoint.transform.position, lastRiverSegment.endPoint.transform.rotation);
                RiverPool[i].SetActive(true);
                lastRiverSegment = RiverPool[i].GetComponent<RiverSegment>();
                return;
            }
        }
    }
    public void RandomizePool()
    {
        for (int i = 0; i < RiverPool.Count; i++)
        {
            GameObject temp = RiverPool[i];
            int randomIndex = Random.Range(i, RiverPool.Count);
            RiverPool[i] = RiverPool[randomIndex];
            RiverPool[randomIndex] = temp;
        }
    }
}