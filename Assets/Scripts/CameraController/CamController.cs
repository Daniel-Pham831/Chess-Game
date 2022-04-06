using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public static CamController Singleton { get; private set; }
    [SerializeField] private GameObject[] cameras;
    private void Awake()
    {
        if (Singleton != null)
            Singleton = this;
    }

    private void Start()
    {
        this.registerEvents(true);
    }

    private void OnDestroy()
    {
        this.registerEvents(false);
    }

    private void registerEvents(bool confirm)
    {
        if (confirm)
        {
            ChessBoard.Singleton.onGameStart += onGameStart;
        }
        else
        {
            ChessBoard.Singleton.onGameStart -= onGameStart;
        }
    }

    private void onGameStart(int team)
    {
        foreach (GameObject camera in cameras)
        {
            camera.SetActive(false);
        }

        cameras[team].SetActive(true);
    }
}
