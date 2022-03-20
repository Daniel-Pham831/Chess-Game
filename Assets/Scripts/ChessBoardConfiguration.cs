using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardConfiguration : MonoBehaviour
{
    //Singleton
    public static ChessBoardConfiguration Singleton { get; private set; }

    [HideInInspector] public readonly int TILE_COUNT_X = 8;
    [HideInInspector] public readonly int TILE_COUNT_Y = 8;
    public int smoothTime = 35;
    public float tileSize = 1f;
    public float yOffset = 0.525f;

    private void Awake()
    {
        Singleton = this;
    }
}
