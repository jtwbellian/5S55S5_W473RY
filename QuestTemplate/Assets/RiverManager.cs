using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class RiverManager : MonoBehaviour
{
    private const float TOL = 1f;
    public bool isHost = false;
    public TextMeshProUGUI messageField;

    public TextMeshPro [] scoreUIP1, scoreUIP2;
    private PhotonView photonView;

    // x is fish, y is clams, z is fruit
    private Vector3 [] playerScores = new Vector3[2];

    #region RiverGeneration

    private const int POOL_SIZE = 7;
    private List<GameObject> riverPool = new List<GameObject>();
    public GameObject [] riverTypes;
    public int numSegments = 5;
    private int progress = 0;
    private RiverSegment turtleSegment = null;

    private string [] riverSegList = new string[POOL_SIZE]; 
    private int currentSegment = 0;

    #endregion

    #region RiverMovement

    //private float spawnDelay = 200f;
    //private float lastSpawnTime = 0;
    private float turnSpeed = 0.25f;
    [SerializeField]
    public Vector3 riverVelocity = Vector3.zero;
    public Vector3 boatRightVelocity = Vector3.zero;

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

    public GameObject startButton, waitMessage, startCanvas, gameEndMenu, scoreBoard;
    public Transform finalScoreBoardSpot;
    public PhotonSpawner [] spawners;

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
        playerScores[0] = Vector3.zero;
        playerScores[1] = Vector3.zero;

        riverMove = false;
        photonView = GetComponent<PhotonView>();

        Physics.IgnoreLayerCollision(10, 10);

        if (PhotonNetwork.IsMasterClient)
        {
            isHost = true;

            //CreateRiverPool();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FishingNet"),
                                        oarSpawnA.position, 
                                        oarSpawnA.rotation, 0);
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FishingNet"),
                                        oarSpawnB.position, 
                                        oarSpawnB.rotation, 0);

            // Generates a random river order and then  shares it with clients
            GenerateRiverList();
            photonView.RPC("SendRiverList", RpcTarget.AllBuffered, (object)riverSegList);
            // Spawn the special pieces
            /* 
            var turtleSegObj = PhotonNetwork.Instantiate(Path.Combine("RiverSegments", "r_turtle"),
                                        oarSpawnB.position, 
                                        oarSpawnB.rotation, 0);

            turtleSegment = turtleSegObj.GetComponent<RiverSegment>();
            turtleSegment.DisableChildObject(false);
            */

            Invoke("TurnOnStartButton", 3f);
            waitMessage.SetActive(false);
        }
        else
        {
            isHost = false;
            waitMessage.SetActive(true);
        }
    }

    private void TurnOnStartButton()
    {
        startButton.SetActive(true);
    }    

    public void AddPoints(int player, int type, int amt)
    {
        Debug.Log("Sending points...");
        photonView.RPC("AddToScore", RpcTarget.AllBuffered, player, type, amt);
    }

    #region PunRPC 

    [PunRPC]
    void SendMessage(string message) 
    {
        messageField.text = message;
    }

    [PunRPC]
    void AddToScore(int player, int type, int amt)
    {
        if (player == -1)
        {
            Debug.Log("Err: No owner given, unsure who to give the points to.");
            return;
        }

        Vector3 score = playerScores[player];
        score[type] = score[type] + amt;
        playerScores[player] = score;

        switch(player)
        {
            case 0:
                scoreUIP1[type].text = (score[type]).ToString();
                break;

            case 1:
                scoreUIP2[type].text = (score[type]).ToString();
                break;
        }
    }

    [PunRPC]
    void SetSegmentActive(int id, bool active)
    {
        GameObject block = riverPool[id];
        block.SetActive(active);
    }

    [PunRPC]
    void SendRiverList(string [] strList)
    {
        riverSegList = strList;
        Debug.Log("Set river list to " + riverSegList);
        // Both sides will create an identical 'River Pool'
        CreateRiverPool();
    }

    [PunRPC]
    void RPC_MoveRudder(float amt)
    {
        boat.rudder += amt;
    }

    [PunRPC]
    void RPC_ReturnToMenu()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
        //SceneManager.LoadScene("Launcher");
    }

    public void ReturnToMenu()
    {
        photonView.RPC("RPC_ReturnToMenu", RpcTarget.AllBuffered);
    }

    public void MoveRudder(float amt)
    {
        photonView.RPC("RPC_MoveRudder", RpcTarget.AllBuffered, amt);
    }

    public void RemoveSeg(int id)
    {
        photonView.RPC("SetSegmentActive", RpcTarget.AllBuffered, id, false);
    }

    // Initializes Our Object Pool of River segments, disabling all at first
    //[PunRPC]
    public void CreateRiverPool()
    {
        // create the River prefabs
        for (int j = 0; j < POOL_SIZE; j++)
        {
            //GameObject block = PhotonNetwork.Instantiate(Path.Combine("RiverSegments", riverTypes[i].name),
            //                        Vector3.zero, 
            //                        Quaternion.identity, 0);

            
            GameObject block;

            if (j == POOL_SIZE - 1)
                block = Instantiate(Resources.Load(Path.Combine("RiverSegments", "r_endzone"))) as GameObject;
            else
                block = Instantiate(Resources.Load(Path.Combine("RiverSegments", riverSegList[j]))) as GameObject;
            
            riverPool.Add(block);
            block.transform.SetParent(riverMaster);

            RiverSegment rs =  block.GetComponent<RiverSegment>();
            
            if (rs == null)
            {
                Debug.Log("River Segment could not be found for \"" + riverSegList[j] + "\"");
                return;
            }

            rs.id = j;
            block.SetActive(false);
            // deactivate river segments on creation
            //rs.DisableChildObject(false);
        }

        // Build first n River segments
        for (int i = 0; i < numSegments; i++)
        {
            AddSeg();
        }
    }
    # endregion

    public void GenerateRiverList()
    {
        var ranStart = Random.Range(0, 100);

        for(int i = 0; i < POOL_SIZE; i ++)
        {
            var indx = (ranStart + i) % (riverTypes.Length - 1);
            riverSegList[i] = riverTypes[indx].name;
            ranStart = Random.Range(0, 100);
            //Debug.Log("Setting element " + i.ToString() + " = " + riverSegList[i].ToString());
        }

        Debug.Log("Generated River: " + riverSegList.ToString());
    }

    public float GetPercentComplete()
    {
        var p = (currentSegment / POOL_SIZE) * 100f;
        Debug.Log(p);

        return p;
    }
    
    public int GetScore(int player, int type)
    {
        Vector3 score = playerScores[player];
        return (int)score[type];
    }

    // Local ONLY, does not change online score.
    // Used during end tally up to save performance
    public void AddToScoreLocal(int player, int type, int amt)
    {
        Vector3 score = playerScores[player];
        score[type] = score[type] + amt;
        playerScores[player] = score;

        switch(player)
        {
            case 0:
                scoreUIP1[type].text = (score[type]).ToString();
                break;

            case 1:
                scoreUIP2[type].text = (score[type]).ToString();
                break;
        }
    }

    /*public void AddRiver()
    {
        // Shuffle the pool to mix it up
        //RandomizePool(); 

        // number of segments before fishing game
        //if (progress >= 15)
        //{
        //    turtleSegment.transform.SetPositionAndRotation(lastRiverSegment.endPoint.transform.position, lastRiverSegment.endPoint.transform.rotation);
        //    turtleSegment.DisableChildObject(true);
        //    lastRiverSegment = turtleSegment;
        //    activeParts ++;
        //    return;
        //}

        // activates the next segment and lays it down
        //for (int i = 0; i < riverPool.Count - 1; i++)
        //{
            //if (!riverPool[i].activeInHierarchy)
            //{
        riverPool[currentSegment].transform.SetPositionAndRotation(lastRiverSegment.endPoint.transform.position, lastRiverSegment.endPoint.transform.rotation);
        
        RiverSegment rs = riverPool[i].GetComponent<RiverSegment>();
        
        if (rs == null)
        {
            Debug.Log("River Segment could not be found");
            return;
        }

        //rs.pv.RPC("Activate", RpcTarget.All, true);
        //riverPool[i].SetActive(true);
        //rs.DisableChildObject(true);
        //targetRotation = lastRiverSegment.endPoint.rotation;
        lastRiverSegment = rs;
        currentSegment ++;
        //activeParts ++;

               // return;
            //}
        //}
    }
*/
    //[PunRPC]
    public void AddSeg()
    {
        if (currentSegment > POOL_SIZE - 1)
            return;

        
        riverPool[currentSegment].transform.SetPositionAndRotation(lastRiverSegment.endPoint.transform.position, lastRiverSegment.endPoint.transform.rotation);

        RiverSegment rs = riverPool[currentSegment].GetComponent<RiverSegment>();
        
        if (rs == null)
        {
            Debug.Log("River Segment could not be found");
            return;
        }

        rs.gameObject.SetActive(true);

        lastRiverSegment = rs;

        if (currentSegment < riverPool.Count)
            currentSegment ++;
    }

    public void Update()
    {
        if (PhotonNetwork.IsMasterClient && riverMove)
        {
            riverVelocity = (Vector3.forward * -levelSpeed) * Time.deltaTime;
            boatRightVelocity = (Vector3.right * boat.rudder) * Time.deltaTime;

            riverMaster.transform.Translate(riverVelocity + boatRightVelocity, Space.World);//((targetSegment.endPoint.position - targetSegment.transform.position).normalized * Time.deltaTime) * levelSpeed;

            var rotAmt = 0f;
            
            if (rotationOffset < TOL)
            {
                rotAmt = Time.deltaTime * levelSpeed;
            }
            else
            if (rotationOffset > TOL)
            {
                rotAmt = Time.deltaTime * levelSpeed * -1;
            }
            else
            {
                rotAmt = 0f;
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
        var fx = FXManager.GetInstance();

        startCanvas.SetActive(false);
        riverMove = true;

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (PhotonSpawner s in spawners)
            {
                s.StartSpawning();
            }
            
            photonView.RPC("SendMessage", RpcTarget.AllBuffered, " ");
        }
        //GetComponent<PhotonView>().RPC("ShowMessage", RpcTarget.AllBuffered, false);
    }

/*
    public void RandomizePool()
    {
        for (int i = 0; i < riverPool.Count; i++)
        {
            GameObject temp = riverPool[i];
            int randomIndex = UnityEngine.Random.Range(i, riverPool.Count);
            riverPool[i] = riverPool[randomIndex];
            riverPool[randomIndex] = temp;
        }
    }*/

    public void GameOver()
    {
        photonView.RPC("RPC_GameOver", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_GameOver()
    {
        ResultsController rc = finalScoreBoardSpot.GetComponent<ResultsController>();

        if (rc)
        {
            rc.ShowMultipliers();
        }

        Invoke("GameEndMenu", 10f);
        scoreBoard.transform.position = finalScoreBoardSpot.position;
        scoreBoard.transform.rotation = finalScoreBoardSpot.rotation;
        scoreBoard.transform.localScale = new Vector3(9f, 5f, 5f);
        FXManager.GetInstance().Burst(FXManager.FX.Confetti2, finalScoreBoardSpot.position + Vector3.right * 1.5f, 15); 
        FXManager.GetInstance().Burst(FXManager.FX.Confetti2, finalScoreBoardSpot.position + Vector3.left * 1.5f, 15); 
        FXManager.GetInstance().Burst(FXManager.FX.Confetti2, finalScoreBoardSpot.position, 15); 
    }

    public void GameEndMenu()
    {
        gameEndMenu.SetActive(true);
    }
}
