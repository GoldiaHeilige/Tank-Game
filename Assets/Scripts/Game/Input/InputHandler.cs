using UnityEngine;
using System;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public Vector2 MouseDelta { get; private set; }
    public bool ScopePressed { get; private set; }
    public bool ZoomPressed { get; private set; }
    public bool RepairPressed { get; private set; }
    public bool ExtinguishPressed { get; private set; }



    public static event Action OnFire;
    private bool isActionBlocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        MouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        ScopePressed = Input.GetMouseButtonDown(1);
        ZoomPressed = Input.GetKeyDown(KeyCode.Z);

        RepairPressed = Input.GetKeyDown(KeyCode.Alpha4);
        ExtinguishPressed = Input.GetKeyDown(KeyCode.Alpha5);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnFire?.Invoke();
        }
    }

    public void BlockInput(bool block)
    {
        isActionBlocked = block;
    }

    public bool IsActionBlocked => isActionBlocked;
}
