using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickController : MonoBehaviour
{
    
    public Transform topOfJoystick;
    public Transform forwardDir;
    public Transform backwardDir;
    public Transform leftDir;
    public Transform rightDir;


    public float forwardTiltAxis = 0;
    public float sideTiltAxis = 0;

    private Transform roverStart;
    private GameObject rover;
    private Rigidbody roverRb;
    
    private Rigidbody topRb;

    [SerializeField] private float roverForceMult = 10f;
    

    void Start()
    {
        topRb = topOfJoystick.gameObject.GetComponent<Rigidbody>();
        rover = GameObject.FindGameObjectWithTag("Rover");
        roverRb = rover.GetComponent<Rigidbody>();
    }

    void Update()
    {

        
        forwardTiltAxis = (Vector3.Distance(backwardDir.position, topOfJoystick.position) - Vector3.Distance(forwardDir.position, topOfJoystick.position))/0.25f;
        sideTiltAxis = (Vector3.Distance(leftDir.position, topOfJoystick.position) - Vector3.Distance(rightDir.position, topOfJoystick.position))/0.25f;
        
        Debug.Log("Forward: " + forwardTiltAxis);
        Debug.Log("Side: " + sideTiltAxis);

        if(forwardTiltAxis > 0.05 || forwardTiltAxis < -0.05)
        {
            roverRb.AddForce(rover.transform.forward*forwardTiltAxis*roverForceMult, ForceMode.Acceleration);
        }
        if(sideTiltAxis > 0.05 || sideTiltAxis < -0.05)
        {
            roverRb.AddForce(rover.transform.right*sideTiltAxis*roverForceMult, ForceMode.Acceleration);
        }
    }

    public void OnHandGrab()
    {
        topRb.constraints = RigidbodyConstraints.None;
        roverRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void OnHandRelease()
    {
        topRb.constraints = RigidbodyConstraints.FreezeRotation;
        roverRb.velocity = Vector3.zero;
    }
}
