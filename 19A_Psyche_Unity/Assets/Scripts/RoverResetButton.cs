using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverResetButton : MonoBehaviour
{

    bool isPressed;

    Transform roverStart;
    public GameObject rover;
    Rigidbody roverRb;

    private Transform unpressed;
    private Transform pressed;
    private Transform button;



    void Start()
    {
        isPressed = false;
        pressed = transform.GetChild(1);
        unpressed = transform.GetChild(2);
        button = transform.GetChild(3);

        roverStart = GameObject.FindGameObjectWithTag("RoverStart").transform;
        //rover = GameObject.FindGameObjectWithTag("Rover");
        roverRb = rover.GetComponent<Rigidbody>();
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
    }

    public void OnButtonRelease()
    {
        isPressed = false;
        button.transform.position = unpressed.position;
    }
}
