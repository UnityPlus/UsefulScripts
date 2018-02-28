using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleMe : MonoBehaviour
{
    public float recycleTime = 3;

    void OnEnable()
    {
        Invoke("Recycle", this.recycleTime);
    }

    void Recycle()
    {
        this.gameObject.GetComponent<RecyclableObject>().Recycle();
    }
}
