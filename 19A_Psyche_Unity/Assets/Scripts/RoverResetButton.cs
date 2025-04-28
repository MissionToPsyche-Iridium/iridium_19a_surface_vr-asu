using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverResetButton : MonoBehaviour
{

    bool isPressed;

    Transform roverStart;
    public GameObject rover;
    Rigidbody roverRb;
    public Checkpoint[] checkpoints;

    private Transform unpressed;
    private Transform pressed;
    private Transform button;
    private GameObject roverCheckpointManager;



    void Start()
    {
        isPressed = false;
        pressed = transform.GetChild(1);
        unpressed = transform.GetChild(2);
        button = transform.GetChild(3);

        roverStart = GameObject.FindGameObjectWithTag("RoverStart").transform;
        //rover = GameObject.FindGameObjectWithTag("Rover");
        roverRb = rover.GetComponent<Rigidbody>();
        roverCheckpointManager = GameObject.FindGameObjectWithTag("CheckpointManager");
    }

    public void OnButtonPress()
    {
        isPressed = true;
        button.transform.position = pressed.position;
        rover.transform.position = roverStart.position;
        rover.transform.rotation = roverStart.rotation;
        roverRb.velocity = Vector3.zero;
        roverRb.constraints = RigidbodyConstraints.FreezeAll;
        rover.SetActive(false);
        roverCheckpointManager.GetComponent<CheckpointManager>().score = 0;
        roverCheckpointManager.GetComponent<CheckpointManager>().timePassed = 0;
        roverCheckpointManager.GetComponent<CheckpointManager>().runTime = false;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].hasTriggered = false;
        }
    }

    public void OnButtonRelease()
    {
        isPressed = false;
        button.transform.position = unpressed.position;
    }
}
