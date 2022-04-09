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
            NetUtility.C_VICTORY_CLAIM += onVictoryClaimClient;
        }
        else
        {
            ChessBoard.Singleton.onGameStart -= onGameStart;
            NetUtility.C_VICTORY_CLAIM -= onVictoryClaimClient;
        }
    }

    private void onGameStart(Team team)
    {
        foreach (GameObject camera in cameras)
        {
            camera.SetActive(false);
        }

        cameras[(int)team + 1].SetActive(true);
    }

    private void onVictoryClaimClient(NetMessage obj)
    {
        foreach (GameObject camera in cameras)
        {
            camera.SetActive(false);
        }

        cameras[0].SetActive(true);
    }
}
