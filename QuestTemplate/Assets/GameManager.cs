using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.IO;

public class GameManager : MonoBehaviour
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
    public static GameManager instance;   
    public Transform oarSpawnA, oarSpawnB;

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
                RiverSegment rs =  block.GetComponent<RiverSegment>();

                if (rs == null)
                {
                    Debug.Log("River Segment could not be found");
                    return;
                }

                rs.pv.RPC("Activate", RpcTarget.All, false);
                //RiverPool[i].SetActive(false);
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

                rs.pv.RPC("Activate", RpcTarget.All, true);
                //RiverPool[i].SetActive(true);

                lastRiverSegment = rs;
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
