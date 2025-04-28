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

    public GameObject controlledObject;

    private GameObject roverCheckpointManager;
    private Rigidbody topRb;
    private Rigidbody controlledObjectRb;
    private ReactToJoystick controlledObjectReact;

    private float forwardTiltAxis = 0;
    private float sideTiltAxis = 0;
    private bool isGrabbed = false;

    [SerializeField] private float joystickDeadzone = 0.05f;
    

    void Start()
    {
        topRb = topOfJoystick.gameObject.GetComponent<Rigidbody>();
        controlledObjectRb = controlledObject.GetComponent<Rigidbody>();
        controlledObjectReact = controlledObject.GetComponent<ReactToJoystick>();
        roverCheckpointManager = GameObject.FindGameObjectWithTag("CheckpointManager");
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            forwardTiltAxis = (Vector3.Distance(backwardDir.position, topOfJoystick.position) - Vector3.Distance(forwardDir.position, topOfJoystick.position)) / 0.25f;
            sideTiltAxis = (Vector3.Distance(leftDir.position, topOfJoystick.position) - Vector3.Distance(rightDir.position, topOfJoystick.position)) / 0.25f;
            controlledObjectReact.AcceptInput(forwardTiltAxis, sideTiltAxis, joystickDeadzone);
        }
    }

    public void OnHandGrab()
    {
        topRb.constraints = RigidbodyConstraints.None;
        controlledObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
        isGrabbed = true;
        roverCheckpointManager.GetComponent<CheckpointManager>().runTime = true;
    }

    public void OnHandRelease()
    {
        isGrabbed = false;
        topRb.constraints = RigidbodyConstraints.FreezeRotation;
        controlledObjectRb.velocity = Vector3.zero;
    }
}
