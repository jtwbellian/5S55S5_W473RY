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
    public GameObject [] RiverTypes;
    public RiverSegment lastRiverSegment;


    #region singleton implementation
    public static GameManager instance;   

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
        CreateRiverPool();
    }
    
    // Initializes Our Object Pool of River segments, disabling all at first
    public void CreateRiverPool()
    {
        // create the River prefabs
        for (int i = 0; i < RiverTypes.Length - 1; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                //GameObject block = Instantiate(RiverTypes[i]);

                GameObject block = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/RiverSegments", "r_" + i.ToString()),
                                        Vector3.zero, 
                                        Quaternion.identity, 0);

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
                RiverPool[i].transform.SetPositionAndRotation(lastRiverSegment.endPoint.transform.position, Quaternion.identity);
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
