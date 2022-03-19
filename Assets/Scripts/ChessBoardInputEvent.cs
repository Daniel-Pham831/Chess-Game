using System;
using UnityEngine;

public class ChessBoardInputEvent : MonoBehaviour
{
    public event Action onLeftMouseButtonDown;
    public event Action onLeftMouseButtonUp;
    public event Action onLeftMouseButtonHold;
    public event Action onRightMouseButtonDown;
    public event Action onRightMouseButtonUp;
    public event Action onRightMouseButtonHold;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (onLeftMouseButtonDown != null)
                onLeftMouseButtonDown();

        if (Input.GetMouseButtonUp(0))
            if (onLeftMouseButtonUp != null)
                onLeftMouseButtonUp();

        if (Input.GetMouseButton(0))
            if (onLeftMouseButtonHold != null)
                onLeftMouseButtonHold();

        if (Input.GetMouseButtonDown(1))
            if (onLeftMouseButtonDown != null)
                onLeftMouseButtonDown();

        if (Input.GetMouseButtonUp(1))
            if (onLeftMouseButtonUp != null)
                onLeftMouseButtonUp();

        if (Input.GetMouseButton(1))
            if (onLeftMouseButtonHold != null)
                onLeftMouseButtonHold();
    }
}