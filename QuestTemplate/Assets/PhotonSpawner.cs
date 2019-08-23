using System.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonSpawner : MonoBehaviour
{
    [Range(0.25f, 4)]
    public float gizmoScale = 1f;

    [SerializeField]
    private int poolSize = 45;
    private float originalDelay;
    private int numActive = 0;
    [ReadOnly]
    public List<GameObject> objPool;
    public float spawnDelay = 100f;
    [Range(0f,100f)]
    public float randomness = 0f;
    public GameObject spawnObj;
    [ReadOnly]
    public bool active = false;
    public float range = 5.2f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (spawnObj == null)
        {
            Gizmos.DrawWireSphere(transform.position, 1f  * gizmoScale);
            return;
        }
        
        var skinnedRenderer = spawnObj.GetComponentInChildren<SkinnedMeshRenderer>();
        var staticRenderer = spawnObj.GetComponentInChildren<MeshFilter>();

        if (skinnedRenderer != null)
        {
            Mesh gizmoMesh = skinnedRenderer.sharedMesh;
            Gizmos.DrawWireMesh(gizmoMesh, 0, transform.position, transform.rotation, skinnedRenderer.transform.localScale * gizmoScale);
        }
        else if (staticRenderer != null)
        {
            Mesh gizmoMesh = staticRenderer.sharedMesh;
            Gizmos.DrawWireMesh(gizmoMesh, 0, transform.position, transform.rotation, staticRenderer.transform.localScale  * gizmoScale);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 1f  * gizmoScale);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        originalDelay = spawnDelay;

        if (PhotonNetwork.IsMasterClient)
        {
            CreateObjPool();
        }
    }

    [ContextMenu("Start")]
    public void StartSpawning()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            active = true;
            StartCoroutine("SpawnUpdate");
        }
    }

    [ContextMenu("Stop")]
    public void StopSpawning()
    {
        active = false;
        // The Coroutine will end itself on the next spawn
    }

    private void CreateObjPool()
    {
        for (int j = 0; j < poolSize; j++)
        {
            GameObject block = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", spawnObj.name),
                            Vector3.zero, 
                            Quaternion.identity, 0);

            objPool.Add(block);

            // Change this part to be a universal "Photon Disableable Class" as soon as we think of a better name
            var pa = block.GetComponent<PhotonActor>();

            if (!pa)
                Debug.Log("No Photon Actor Script found");

            pa.DisableChildObject(false); // Deactivate the fish on creation
        } 
    }

    private void AddObj(Vector3 position)
    {
        // Finds the first inactive fish and returns gameObject
        for (int i = 0; i < objPool.Count - 1; i++)
        {
            if (!objPool[i].activeInHierarchy)
            {
                //objPool[i].transform.SetPositionAndRotation(position, transform.rotation);
                
                // Change this part to be a universal "Photon Disableable Script" as soon as we think of a better name
                PhotonActor pa = objPool[i].GetComponent<PhotonActor>();
                
                if (pa == null)
                {
                    Debug.Log("Fish instance could not be found. Null or unknown entity in pool!");
                    return;
                }

                // Set this fish active
                pa.DisableChildObject(true);
                pa.SetPositionAndRotation(position, transform.rotation);
                pa.SetForces(true, Vector3.zero, Vector3.zero);

                PhotonFish pf = pa.gameObject.GetComponent<PhotonFish>();

                if (pf) // If it is a fish
                {
                    pf.ResetSpawnSettings();
                }

                FruitTree ft = pa.gameObject.GetComponent<FruitTree>();
                
                if (ft)
                {
                    ft.Invoke("ChildToRiver", 0.25f);
                }

                return;
            }
        }
    }

    private IEnumerator SpawnUpdate()
    {
        while (true)
        {
            Vector3 ranPos = new Vector3(Random.Range(-range, range), transform.position.y, transform.position.z);
            AddObj(ranPos);

            // At 100 randomness, delay is between be one second or double the original time
            spawnDelay = Mathf.Max(1, originalDelay + Random.Range(-originalDelay * (randomness/100f), originalDelay * (randomness/100f)));

            if (!active) // Break coroutine if not active
            {
                yield break;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
