using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleMaster : MonoBehaviour {
    #region Members
    #region Static Instance
    private static RecycleMaster recycleMasterInstance = null;

    public static RecycleMaster instance
    {
        get
        {
            if (recycleMasterInstance == null)
            {
                recycleMasterInstance = FindObjectOfType(typeof(RecycleMaster)) as RecycleMaster;
            }

            if (recycleMasterInstance == null)
            {
                GameObject newObj = new GameObject("RecycleMaster");
                recycleMasterInstance = newObj.AddComponent(typeof(RecycleMaster)) as RecycleMaster;
                Debug.Log("Could not find RecycleMaster, so I made one");
            }

            return recycleMasterInstance;
        }
    }
    #endregion

    private Dictionary<int, ObjectRecycler> recyclerDictionary = new Dictionary<int, ObjectRecycler>();

    /// <summary>
    /// The recyclable object dictionary, used when this is passed a non-recycleable Object (bad people...)
    /// </summary>
    private Dictionary<GameObject, int> recyclableObjectDictionary = new Dictionary<GameObject, int>();
    #endregion

    #region Methods
    public void Awake()
    {
        Object.DontDestroyOnLoad(this.gameObject);
    }

    private bool IsRecycleable(GameObject objectToGet)
    {
        RecyclableObject ro = objectToGet.GetComponent<RecyclableObject>();

        if (ro == null)
            return false;
        else
            return true;
    }

    private GameObject GetNextFreeObject(GameObject objectToGet)
    {
        if (!IsRecycleable(objectToGet))//Make sure it has an ID
            this.MakeObjectRecycleable(objectToGet);

        int id = this.GetRecyclerID(objectToGet);//Get the ID so we can get the corresponding Recycler

        ObjectRecycler oR = this.GetRecycler(id, objectToGet);//Get the Recycler, send GO incase we need to make a new one

        GameObject tempGO = oR.NextFreeObject;

        this.AttachRecyclableObjectScript(tempGO, id);

        return tempGO;//Return nextFree!
    }

    private void MakeObjectRecycleable(GameObject goToRecycle)
    {
        if (!this.recyclableObjectDictionary.ContainsKey(goToRecycle))
            this.recyclableObjectDictionary.Add(goToRecycle, this.GetUniqueRecyleID());
    }

    private int GetRecyclerID(GameObject goToGet)
    {
        if (this.recyclableObjectDictionary.ContainsKey(goToGet))
        {
            return this.recyclableObjectDictionary[goToGet];
        }
        else
        {
            RecyclableObject ro = goToGet.GetComponent<RecyclableObject>();

            return ro.recycleID;
        }
    }

    //Gets a Recycler. If there isn't one, make it!
    private ObjectRecycler GetRecycler(int recyclerID, GameObject goToMake)
    {
        if (this.recyclerDictionary.ContainsKey(recyclerID))
            return this.recyclerDictionary[recyclerID];
        else
        {
            ObjectRecycler newOR = new ObjectRecycler(goToMake, 0);

            this.recyclerDictionary.Add(recyclerID, newOR);

            return newOR;
        }
    }

    private void AttachRecyclableObjectScript(GameObject goToAttachTo, int idToUse)
    {
        if (goToAttachTo.GetComponentInChildren<RecyclableObject>() == null)
        {
            RecyclableObject rO = goToAttachTo.AddComponent<RecyclableObject>();
            rO.recycleID = idToUse;
        }
    }
    #endregion

    #region Events
    public GameObject GetFreeObject(GameObject objectPrefab)
    {
        return this.GetNextFreeObject(objectPrefab);
    }

    public void RecycleObject(RecyclableObject objectToRecycle)
    {
        if (this.recyclerDictionary.ContainsKey(objectToRecycle.recycleID))
            this.recyclerDictionary[objectToRecycle.recycleID].RecycleObject(objectToRecycle.gameObject);
        else
            Debug.Log("You are trying to free an object w/o a recycler. You are looking for id: " + objectToRecycle.recycleID);
    }
    #endregion

    #region Helpers
    public int GetUniqueRecyleID()
    {
        return System.DateTime.Now.Millisecond + System.DateTime.Now.Second + (int)System.DateTime.Now.Ticks + (int)Time.timeSinceLevelLoad;
    }
    #endregion
}
