using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Could easily have been an attribute of a PhotonActor base class or anything else, but here we are
public class Item : MonoBehaviour
{
    public int owner = -1;
    public int type = -1;
}
