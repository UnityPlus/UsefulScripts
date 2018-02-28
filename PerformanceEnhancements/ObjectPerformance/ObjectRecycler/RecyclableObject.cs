using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclableObject : MonoBehaviour
{
    /// <summary>
    /// The recycle ID. MUST be unique. 
    /// </summary>
    public int recycleID = 0;


    /// <summary>
    /// Frees the object. Use this instead of Destroy
    /// </summary>
    public void Recycle()
    {
        RecycleMaster.instance.RecycleObject(this);
    }
}
