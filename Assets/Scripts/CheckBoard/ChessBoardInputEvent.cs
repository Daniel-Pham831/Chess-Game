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
            this.onLeftMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(0))
            this.onLeftMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(0))
            this.onLeftMouseButtonHold?.Invoke();

        if (Input.GetMouseButtonDown(1))
            this.onRightMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(1))
            this.onRightMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(1))
            this.onRightMouseButtonHold?.Invoke();
    }
}