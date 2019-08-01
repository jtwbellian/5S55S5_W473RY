using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.IO;

public class RiverManager : MonoBehaviour
{
    #region RiverGeneration

    private const int POOL_SIZE = 6;
    private List<GameObject> riverPool = new List<GameObject>();
    public GameObject [] riverTypes;
    public int numSegments = 5;

    #endregion

    #region RiverMovement

    //private float spawnDelay = 200f;
    //private float lastSpawnTime = 0;
    private float turnSpeed = 0.25f;
    [SerializeField]
    private Vector3 riverVelocity = Vector3.zero;
    private Vector3 boatRightVelocity = Vector3.zero;

    public float rotationOffset = 0;
    public bool riverMove = false;

    [Range(0f, 10f)]
    public float levelSpeed = 0f;
    public RiverSegment lastRiverSegment;

    public Transform riverMaster;
    public RiverSegment targetSegment;

    public int activeParts = 2;
    public Boat boat;

    #endregion

    public int score = 0;
    public int lives = 3;
    public Transform oarSpawnA, oarSpawnB;
    public bool gameOver = false;

    public GameObject startButton, waitMessage;

    #region singleton implementation

    public static RiverManager instance;   

   // public vector3 current

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
        riverMove = false;

        if (PhotonNetwork.IsMasterClient)
        {
            CreateRiverPool();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FishingNet"),
                                        oarSpawnA.position, 
                                        oarSpawnA.rotation, 0);
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FishingNet"),
                                        oarSpawnB.position, 
                                        oarSpawnB.rotation, 0);

            Invoke("TurnOnStartButton", 3f);
            waitMessage.SetActive(false);
        }
        else
        {
            waitMessage.SetActive(true);
        }
    }

    private void TurnOnStartButton()
    {
        startButton.SetActive(true);
    }    
    // Initializes Our Object Pool of River segments, disabling all at first
    public void CreateRiverPool()
    {
        // create the River prefabs
        for (int i = 0; i < riverTypes.Length; i++)
        {
            for (int j = 0; j < POOL_SIZE; j++)
            {
                GameObject block = PhotonNetwork.Instantiate(Path.Combine("RiverSegments", riverTypes[i].name),
                                        Vector3.zero, 
                                        Quaternion.identity, 0);

                riverPool.Add(block);
                block.transform.SetParent(riverMaster);

                RiverSegment rs =  block.GetComponent<RiverSegment>();

                if (rs == null)
                {
                    Debug.Log("River Segment could not be found");
                    return;
                }
                // deactivate river segments on creation
                rs.DisableChildObject(false);
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
        for (int i = 0; i < riverPool.Count - 1; i++)
        {
            if (!riverPool[i].activeInHierarchy)
            {
                riverPool[i].transform.SetPositionAndRotation(lastRiverSegment.endPoint.transform.position, lastRiverSegment.endPoint.transform.rotation);
                
                RiverSegment rs = riverPool[i].GetComponent<RiverSegment>();
                
                if (rs == null)
                {
                    Debug.Log("River Segment could not be found");
                    return;
                }

                //rs.pv.RPC("Activate", RpcTarget.All, true);
                //riverPool[i].SetActive(true);
                rs.DisableChildObject(true);
                //targetRotation = lastRiverSegment.endPoint.rotation;
                lastRiverSegment = rs;
                activeParts ++;

                return;
            }
        }
    }

    
    public void Update()
    {
        if (PhotonNetwork.IsMasterClient && riverMove)
        {
            riverVelocity = (Vector3.forward * -levelSpeed) * Time.deltaTime;
            boatRightVelocity = (Vector3.right * boat.rudder) * Time.deltaTime;

            riverMaster.transform.Translate(riverVelocity + boatRightVelocity, Space.World);//((targetSegment.endPoint.position - targetSegment.transform.position).normalized * Time.deltaTime) * levelSpeed;

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

/*
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
    */

    public void StartRiver()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(false);
            riverMove = true;
        }
        else
        {
            waitMessage.SetActive(false);
        }
    }

    public void RandomizePool()
    {
        for (int i = 0; i < riverPool.Count; i++)
        {
            GameObject temp = riverPool[i];
            int randomIndex = Random.Range(i, riverPool.Count);
            riverPool[i] = riverPool[randomIndex];
            riverPool[randomIndex] = temp;
        }
    }
}
