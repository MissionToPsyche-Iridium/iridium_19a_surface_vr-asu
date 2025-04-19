using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToJoystick : MonoBehaviour
{

    private Rigidbody rb;

    [SerializeField] private bool isRover;
    [SerializeField] private float forceMultiplier = 10f;

    [SerializeField] private float rotationStrength = 5f;

    [SerializeField] private GameObject[] roverWheels;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AcceptInput(float forwardTiltAxis, float sideTiltAxis, float joystickDeadzone)
    {
        if (isRover)
        {
            gameObject.SetActive(true);
            
            if (sideTiltAxis > joystickDeadzone || sideTiltAxis < -joystickDeadzone)
            {
                //rb.AddForce(transform.right * sideTiltAxis * forceMultiplier, ForceMode.Force);
                transform.Rotate(transform.up, rotationStrength*sideTiltAxis);
                for (int i = 0; i < roverWheels.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        roverWheels[i].transform.Rotate(Vector3.right, sideTiltAxis * 20);
                    }
                    else
                    {
                        roverWheels[i].transform.Rotate(Vector3.right, -sideTiltAxis * 20);
                    }
                }
            }
            if (forwardTiltAxis > joystickDeadzone || forwardTiltAxis < -joystickDeadzone)
            {
                rb.AddForce(transform.forward * forwardTiltAxis * forceMultiplier, ForceMode.Force);
                for (int i = 0; i < roverWheels.Length; i++)
                {
                    roverWheels[i].transform.Rotate(Vector3.right, forwardTiltAxis * 10 * Mathf.Abs(forwardTiltAxis));
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject);
    }
}
