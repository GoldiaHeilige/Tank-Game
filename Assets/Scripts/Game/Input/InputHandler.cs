using UnityEngine;
using System;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public Vector2 MouseDelta { get; private set; }

    public static event Action OnFire;

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

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnFire?.Invoke();
        }
    }
}
