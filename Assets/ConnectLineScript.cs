using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectLineScript : MonoBehaviour
{
    public GameObject server;
    public GameObject capsule;
    public GameObject left;
    public GameObject right;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            server.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Keypad1))
            capsule.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            left.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            right.SetActive(true);

    }
}
