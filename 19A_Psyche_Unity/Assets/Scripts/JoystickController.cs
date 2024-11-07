using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickController : MonoBehaviour
{
    
    public Transform topOfJoystick;
    public Transform anchor;
    public Transform leftHand;
    public Transform rightHand;
    public Transform forwardDir;
    public Transform backwardDir;
    public Transform leftDir;
    public Transform rightDir;


    [SerializeField] private float forwardTiltAxis = 0;
    [SerializeField] private float sideTiltAxis = 0;

    
    private Rigidbody topRb;
    private Quaternion startingRot;

    private bool leftHandGrabbed = false;
    private bool rightHandGrabbed = false;
    

    void Start()
    {
        topRb = topOfJoystick.gameObject.GetComponent<Rigidbody>();
        startingRot = anchor.localRotation;
    }

    void Update()
    {

        
        forwardTiltAxis = Vector3.Angle(topOfJoystick.up, anchor.position);

        sideTiltAxis = Vector3.Angle(topOfJoystick.right, anchor.position);
        
        if(leftHandGrabbed)
        {
            
        }
        if(rightHandGrabbed)
        {
            
        }
        //Debug.Log("Forward: " + forwardTiltAxis);
        //Debug.Log("Side: " + sideTiltAxis);
        Debug.Log("Forward: " + Vector3.Distance(forwardDir.position, topOfJoystick.position));
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
