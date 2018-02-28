using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRecyclableObjectSpawner : MonoBehaviour {

    public GameObject cube;
    public GameObject sphere;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject go = RecycleMaster.instance.GetFreeObject(cube);
            go.transform.position = this.transform.position;
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject go = RecycleMaster.instance.GetFreeObject(sphere);
            go.transform.position = this.transform.position;
        }
    }
}
