using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickController : MonoBehaviour
{
    
    public Transform topOfJoystick;
    public Transform leftHand;
    public Transform rightHand;

    [SerializeField] private float forwardTiltAxis = 0;
    [SerializeField] private float sideTiltAxis = 0;

    private Vector3 anchor;
    private Rigidbody topRb;

    private bool leftHandGrabbed = false;
    private bool rightHandGrabbed = false;
    

    void Start()
    {
        topRb = topOfJoystick.gameObject.GetComponent<Rigidbody>();
        anchor = topOfJoystick.GetComponent<CharacterJoint>().anchor;
    }

    void Update()
    {
        forwardTiltAxis = Vector3.Angle(anchor, new Vector3(topOfJoystick.position.x, topOfJoystick.position.y, 0));
        if(forwardTiltAxis < 355 && forwardTiltAxis > 290)
        {
            forwardTiltAxis = Mathf.Abs(forwardTiltAxis -360);
            Debug.Log("Backward: " + forwardTiltAxis);
        }
        else if(forwardTiltAxis > 5 && forwardTiltAxis < 74)
        {
            Debug.Log("Forward: " + forwardTiltAxis);
        }

        sideTiltAxis = Vector3.Angle(anchor, new Vector3(0, topOfJoystick.position.y, topOfJoystick.position.z));
        if(sideTiltAxis < 355 && sideTiltAxis > 290)
        {
            sideTiltAxis = Mathf.Abs(sideTiltAxis -360);
            Debug.Log("Right: " + sideTiltAxis);
        }
        else if(sideTiltAxis > 5 && sideTiltAxis < 74)
        {
            Debug.Log("Left: " + sideTiltAxis);
        }
        if(leftHandGrabbed)
        {
            
        }
        if(rightHandGrabbed)
        {
            
        }
        Debug.Log("Forward: " + forwardTiltAxis);
        Debug.Log("Side: " + sideTiltAxis);
    }

    public void OnHandGrab()
    {
        if(Vector3.Distance(topOfJoystick.position, leftHand.position) < Vector3.Distance(topOfJoystick.position, rightHand.position))
        {
            leftHandGrabbed = true;
            rightHandGrabbed = false;
        }
        else
        {
            leftHandGrabbed = false;
            rightHandGrabbed = true;
        }
        topRb.constraints = RigidbodyConstraints.None;
    }

    public void OnHandRelease()
    {
        leftHandGrabbed = false;
        rightHandGrabbed = false;
        topRb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
