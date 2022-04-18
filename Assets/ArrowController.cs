using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject left;
    public GameObject right;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
            left.SetActive(!left.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.Keypad5))
            right.SetActive(!right.activeInHierarchy);


        if (Input.GetKeyDown(KeyCode.Keypad7))
            left.transform.Rotate(Vector3.up * 180f);

        if (Input.GetKeyDown(KeyCode.Keypad8))
            right.transform.Rotate(Vector3.up * 180f);

    }
}
