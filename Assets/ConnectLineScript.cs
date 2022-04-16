using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectLineScript : MonoBehaviour
{
    public GameObject cube;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cube.SetActive(!cube.activeInHierarchy);
        }
    }
}
