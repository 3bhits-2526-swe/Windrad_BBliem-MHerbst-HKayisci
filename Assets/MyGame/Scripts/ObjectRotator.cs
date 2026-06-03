using UnityEngine;


public class ObjectRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;  // Degrees per second
    [Space]
    [Space]
    [Space]
    [Header("Wer das liest is ein Esel")]
    [Space]
    [Space]
    [Space]
    [Header("Rotation Axes")]
    [SerializeField] private bool rotateAroundX = true;   // W/S or Up/Down
    [SerializeField] private bool rotateAroundY = true;   // A/D or Left/Right
    [SerializeField] private bool rotateAroundZ = false;  // Optional: Q/E

    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        float rotationX = 0f;
        float rotationY = 0f;
        float rotationZ = 0f;

        // Arrow Keys Input
        // Up/Down Arrow = Rotate around X axis
        if (Input.GetKey(KeyCode.UpArrow))
            rotationX = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            rotationX = -rotationSpeed * Time.deltaTime;

        // Left/Right Arrow = Rotate around Y axis (yaw)
        if (Input.GetKey(KeyCode.LeftArrow))
            rotationY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            rotationY = rotationSpeed * Time.deltaTime;

        // WASD Input
        // W/S = Rotate around X axis (pitch)
        if (Input.GetKey(KeyCode.W))
            rotationX = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            rotationX = -rotationSpeed * Time.deltaTime;

        // A/D = Rotate around Y axis (yaw)
        if (Input.GetKey(KeyCode.A))
            rotationY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            rotationY = rotationSpeed * Time.deltaTime;

        // Q/E = Rotate around Z axis (roll) - optional
        if (Input.GetKey(KeyCode.Q))
            rotationZ = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            rotationZ = -rotationSpeed * Time.deltaTime;

        // Apply rotation
        if (rotateAroundX)
            transform.Rotate(rotationX, 0, 0, Space.Self);
        if (rotateAroundY)
            transform.Rotate(0, rotationY, 0, Space.Self);
        if (rotateAroundZ)
            transform.Rotate(0, 0, rotationZ, Space.Self);
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
}
