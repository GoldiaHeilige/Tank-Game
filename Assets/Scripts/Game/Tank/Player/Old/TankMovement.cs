using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public Rigidbody rb;
    public TankData tankData;

    private float moveInput;
    private float turnInput;

    private float currentMoveSpeed = 0f;
    private float currentTurnSpeed = 0f;

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");    // W/S
        turnInput = Input.GetAxis("Horizontal");  // A/D
    }

    void FixedUpdate()
    {
        // 🚫 KHÔNG di chuyển nếu xe bị lật (góc nghiêng quá lớn)
        if (Vector3.Dot(transform.up, Vector3.up) < 0.5f)
            return;

        // === TÍNH GIA TỐC DI CHUYỂN ===
        float targetMove = moveInput * tankData.moveForce;
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, targetMove, tankData.moveAcceleration * Time.fixedDeltaTime);

        // === TÍNH GIA TỐC XOAY ===
        float targetTurn = turnInput * tankData.turnForce;
        currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, targetTurn, tankData.turnAcceleration * Time.fixedDeltaTime);

        // === ÁP DỤNG LỰC ===
        Vector3 moveVector = transform.forward * currentMoveSpeed;
        rb.MovePosition(rb.position + moveVector * Time.fixedDeltaTime);

        Quaternion turnRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
