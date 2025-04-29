using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public Rigidbody MarsGrav; // Mars gravity sphere
    public Rigidbody PsycheGrav; // Psyche gravity sphere

    // Earth gravity constant
    private Vector3 earthGravity = new Vector3(0, -9.81f, 0);

    // Scale factors for Mars and Psyche gravity (38% and 14% of Earth's gravity)
    private float marsGravityScale = 0.38f;
    private float psycheGravityScale = 0.14f;

    void Start()
    {

        if (MarsGrav != null)
        {
            // Mars gravity (38% of Earth's gravity)
            MarsGrav.useGravity = false;  // Disable Unity's default gravity
        }

        if (PsycheGrav != null)
        {
            // Psyche gravity (14% of Earth's gravity)
            PsycheGrav.useGravity = false;  // Disable Unity's default gravity
        }
    }

    void FixedUpdate()
    {
        // Apply custom gravity

        if (MarsGrav != null)
        {
            // Apply Mars gravity (38% of Earth gravity)
            MarsGrav.AddForce(earthGravity * marsGravityScale, ForceMode.Acceleration);
        }

        if (PsycheGrav != null)
        {
            // Apply Psyche gravity (14% of Earth gravity)
            PsycheGrav.AddForce(earthGravity * psycheGravityScale, ForceMode.Acceleration);
        }
    }
}