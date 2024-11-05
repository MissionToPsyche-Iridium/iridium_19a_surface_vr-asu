using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    
    public Transform topOfJoystick;

    [SerializeField] private float forwardTiltAxis = 0;
    [SerializeField] private float sideTiltAxis = 0;
    

    void Update()
    {
        forwardTiltAxis = topOfJoystick.rotation.eulerAngles.x;
        if(forwardTiltAxis < 355 && forwardTiltAxis > 290)
        {
            forwardTiltAxis = Mathf.Abs(forwardTiltAxis -360);
            Debug.Log("Backward: " + forwardTiltAxis);
        }
        else if(forwardTiltAxis > 5 && forwardTiltAxis < 74)
        {
            Debug.Log("Forward: " + forwardTiltAxis);
        }

        sideTiltAxis = topOfJoystick.rotation.eulerAngles.z;
        if(sideTiltAxis < 355 && sideTiltAxis > 290)
        {
            sideTiltAxis = Mathf.Abs(sideTiltAxis -360);
            Debug.Log("Right: " + sideTiltAxis);
        }
        else if(sideTiltAxis > 5 && sideTiltAxis < 74)
        {
            Debug.Log("Left: " + sideTiltAxis);
        }
    }
}
