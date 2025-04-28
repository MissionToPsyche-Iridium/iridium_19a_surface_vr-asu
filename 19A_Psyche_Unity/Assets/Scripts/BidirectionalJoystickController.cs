using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BidirectionalJoystickController : MonoBehaviour
{
    public Transform topOfJoystick;
    public Transform positiveDir;
    public Transform negativeDir;

    public GameObject controlledObject;
    private GameObject roverCheckpointManager;

    private Rigidbody topRb;
    private Rigidbody controlledObjectRb;
    private ReactToJoystick controlledObjectReact;

    private float tiltAxis = 0;
    private bool isGrabbed = false;

    private Quaternion startingRot;

    [SerializeField] private float joystickDeadzone = 0.05f;
    [SerializeField] bool isLeftRight = false;


    void Start()
    {
        
        topRb = topOfJoystick.gameObject.GetComponent<Rigidbody>();
        startingRot = topOfJoystick.rotation;
        controlledObjectRb = controlledObject.GetComponent<Rigidbody>();
        controlledObjectReact = controlledObject.GetComponent<ReactToJoystick>();
        roverCheckpointManager = GameObject.FindGameObjectWithTag("CheckpointManager");
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            tiltAxis = (Vector3.Distance(negativeDir.position, topOfJoystick.position) - Vector3.Distance(positiveDir.position, topOfJoystick.position)) / 0.25f;
            if (!isLeftRight)
            {
                controlledObjectReact.AcceptInput(tiltAxis, 0, joystickDeadzone);
            }
            else
            {
                controlledObjectReact.AcceptInput(0, tiltAxis, joystickDeadzone);
            }
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
        topOfJoystick.rotation = startingRot;
        topRb.constraints = RigidbodyConstraints.FreezeRotation;
        controlledObjectRb.velocity = Vector3.zero;
    }
}
