using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToJoystick : MonoBehaviour
{

    private Rigidbody rb;

    [SerializeField] private bool isRover;
    [SerializeField] private float forceMultiplier = 10f;

    [SerializeField] private float rotationStrength = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AcceptInput(float forwardTiltAxis, float sideTiltAxis, float joystickDeadzone)
    {
        if (isRover)
        {
            gameObject.SetActive(true);
            if (forwardTiltAxis > joystickDeadzone || forwardTiltAxis < -joystickDeadzone)
            {
                rb.AddForce(transform.forward * forwardTiltAxis * forceMultiplier, ForceMode.Force);
            }
            if (sideTiltAxis > joystickDeadzone || sideTiltAxis < -joystickDeadzone)
            {
                //rb.AddForce(transform.right * sideTiltAxis * forceMultiplier, ForceMode.Force);
                transform.Rotate(transform.up, rotationStrength*sideTiltAxis);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject);
    }
}
