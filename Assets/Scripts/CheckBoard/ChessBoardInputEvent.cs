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
            if (this.onLeftMouseButtonDown != null)
                this.onLeftMouseButtonDown();

        if (Input.GetMouseButtonUp(0))
            if (this.onLeftMouseButtonUp != null)
                this.onLeftMouseButtonUp();

        if (Input.GetMouseButton(0))
            if (this.onLeftMouseButtonHold != null)
                this.onLeftMouseButtonHold();

        if (Input.GetMouseButtonDown(1))
            if (this.onRightMouseButtonDown != null)
                this.onRightMouseButtonDown();

        if (Input.GetMouseButtonUp(1))
            if (this.onRightMouseButtonUp != null)
                this.onRightMouseButtonUp();

        if (Input.GetMouseButton(1))
            if (this.onRightMouseButtonHold != null)
                this.onRightMouseButtonHold();
    }
}