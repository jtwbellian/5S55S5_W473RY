﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.IO;

public class GameManager : MonoBehaviour
{
    private List<GameObject> RiverPool = new List<GameObject>();
    [SerializeField]
    private float turnSpeed = 0.25f;
    public float rotationOffset = 0;
    public bool riverMove = false;

    [Range(0f, 10f)]
    public float levelSpeed = 0f;

    public int score = 0;
    public int lives = 3;

    public int numSegments = 5;
    public bool gameOver = false;
    public string [] RiverTypes;
    public RiverSegment lastRiverSegment;

    #region singleton implementation
    public static GameManager instance;   
    public Transform oarSpawnA, oarSpawnB, boat;
    public Transform riverMaster;
    public RiverSegment targetSegment;

    private float spawnDelay = 200f;
    private float lastSpawnTime = 0;

   // public vector3 current

    void Awake()
    {
        if (GameManager.instance != this)
        {
            GameManager.instance = this;
        }
    }
#endregion

    // Start is called before the first frame update
    void Start()
    {
        riverMove = false;

        if (PhotonNetwork.IsMasterClient)
        {
            CreateRiverPool();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Paddle"),
                                        oarSpawnA.position, 
                                        oarSpawnA.rotation, 0);
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Paddle"),
                                        oarSpawnB.position, 
                                        oarSpawnB.rotation, 0);
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
                GameObject block = PhotonNetwork.Instantiate(Path.Combine("RiverSegments", RiverTypes[i]),
                                        Vector3.zero, 
                                        Quaternion.identity, 0);

                RiverPool.Add(block);
                block.transform.SetParent(riverMaster);

                RiverSegment rs =  block.GetComponent<RiverSegment>();

                //rs.pv.RPC("Activate", RpcTarget.All, false);
                rs.DisableChildObject(false);

                if (rs == null)
                {
                    Debug.Log("River Segment could not be found");
                    return;
                }
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
                
                RiverSegment rs = RiverPool[i].GetComponent<RiverSegment>();
                
                if (rs == null)
                {
                    Debug.Log("River Segment could not be found");
                    return;
                }

                //rs.pv.RPC("Activate", RpcTarget.All, true);
                //RiverPool[i].SetActive(true);
                rs.DisableChildObject(true);
                //targetRotation = lastRiverSegment.endPoint.rotation;
                lastRiverSegment = rs;
                return;
            }
        }
    }

    
    public void Update()
    {
        if (PhotonNetwork.IsMasterClient && riverMove)
        {
            riverMaster.transform.Translate((Vector3.forward * Time.deltaTime) * -levelSpeed, Space.World);//((targetSegment.endPoint.position - targetSegment.transform.position).normalized * Time.deltaTime) * levelSpeed;

            var rotAmt = 0f;
            
            if (rotationOffset < 0)
            {
                rotAmt = Time.deltaTime * levelSpeed;
            }
            
            if (rotationOffset > 0)
            {
                rotAmt = Time.deltaTime * levelSpeed * -1;
            }

            rotationOffset += rotAmt;

            var x = riverMaster.root.rotation.eulerAngles.x;
            var y = riverMaster.root.rotation.eulerAngles.y + rotAmt;
            var z = riverMaster.root.rotation.eulerAngles.z;

            riverMaster.root.rotation = Quaternion.Euler(x, y, z);

            //if (targetSegment != null)
            //riverMaster.rotation = RotatePointAroundPivot(riverMaster.transform.position, boat.transform.position, targetSegment.endPoint.rotation.toEulers());
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
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
