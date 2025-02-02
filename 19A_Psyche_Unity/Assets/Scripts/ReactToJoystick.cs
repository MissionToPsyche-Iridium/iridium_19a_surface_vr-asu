using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToJoystick : MonoBehaviour
{

    private Rigidbody rb;

    [SerializeField] private bool isRover;
    [SerializeField] private float forceMultiplier = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AcceptInput(float forwardTiltAxis, float sideTiltAxis, float joystickDeadzone)
    {
        if (isRover)
        {
            if (forwardTiltAxis > joystickDeadzone || forwardTiltAxis < -joystickDeadzone)
            {
                rb.AddForce(transform.forward * forwardTiltAxis * forceMultiplier, ForceMode.Acceleration);
            }
            if (sideTiltAxis > joystickDeadzone || sideTiltAxis < -joystickDeadzone)
            {
                rb.AddForce(transform.right * sideTiltAxis * forceMultiplier, ForceMode.Acceleration);
            }
        }
    }
}
