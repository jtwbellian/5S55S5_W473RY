using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Photon;

// Photon Player represents a unique person online in the session
public class PhotonPlayer : MonoBehaviour
{
    PhotonView PV;
    public GameObject myAvatar;
    public AvatarController avController; // Controller for the avatar animations
    private AvatarParts parts; // Head and paw gameObjects
    
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Debug.Log("PV set to " + PV);

        if (PV.IsMine) // Only setup if PhotonView belongs to current player
        {
            if (PhotonNetwork.IsMasterClient) // Player 1
            {
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar1"),
                                                    Vector3.zero, 
                                                    Quaternion.identity, 0);
            }
            else // Player 2
            {
                    myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar2"),
                                        Vector3.zero, 
                                        Quaternion.identity, 0);
            }

            // Notice the OVRPlayer controller is NOT created using PhotonNetwork.Instantiate 
            // this is because it should be local- the avatar parts are synchronized on Photon,
            // so they need to be childed to the players HMD and controllers on one side of the server

            GameObject obj = Resources.Load<GameObject>("PhotonPrefabs/OVRPlayerController");  
            var player = Instantiate(obj);

            // Assigning the avatar parts
            parts = myAvatar.GetComponent<AvatarParts>();
            avController = player.GetComponent<AvatarController>();

            parts.Head.SetParent(avController.headTarget.transform);
            parts.Head.localRotation = Quaternion.identity;
            parts.Head.localPosition = Vector3.zero;

            parts.RHand.SetParent(avController.rhandTarget.transform);
            parts.RHand.localRotation = Quaternion.identity;
            parts.RHand.localPosition = Vector3.zero;

            parts.LHand.SetParent(avController.lhandTarget.transform);
            parts.LHand.localRotation = Quaternion.identity;
            parts.LHand.localPosition = Vector3.zero;
        }
    }
}
