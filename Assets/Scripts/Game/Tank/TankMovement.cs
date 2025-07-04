using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    public TankData tankData; // Gán SO vào đây

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = InputHandler.Instance.Vertical;
        float turnInput = InputHandler.Instance.Horizontal;

        Vector3 move = transform.forward * moveInput * tankData.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        Quaternion turn = Quaternion.Euler(0f, turnInput * tankData.turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }

}
