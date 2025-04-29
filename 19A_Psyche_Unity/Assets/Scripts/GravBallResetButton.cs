using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravBallResetButton : MonoBehaviour
{
    bool isPressed;

    public GameObject earthGrav;
    public GameObject marsGrav;
    public GameObject psycheGrav;

    private Rigidbody earthRb;
    private Rigidbody marsRb;
    private Rigidbody psycheRb;

    private Transform earthStart;
    private Transform marsStart;
    private Transform psycheStart;

    private Transform pressed;
    private Transform unpressed;
    private Transform button;

    void Start()
    {
        isPressed = false;

        // Setup button visuals
        pressed = transform.GetChild(1);
        unpressed = transform.GetChild(2);
        button = transform.GetChild(3);

        // Setup starting positions
        earthStart = new GameObject("EarthStart").transform;
        earthStart.position = earthGrav.transform.position;
        marsStart = new GameObject("MarsStart").transform;
        marsStart.position = marsGrav.transform.position;
        psycheStart = new GameObject("PsycheStart").transform;
        psycheStart.position = psycheGrav.transform.position;

        // Get rigidbodies
        earthRb = earthGrav.GetComponent<Rigidbody>();
        marsRb = marsGrav.GetComponent<Rigidbody>();
        psycheRb = psycheGrav.GetComponent<Rigidbody>();
    }

    public void OnButtonPress()
    {
        isPressed = true;
        button.transform.position = pressed.position;

        ResetBall(earthGrav, earthRb, earthStart);
        ResetBall(marsGrav, marsRb, marsStart);
        ResetBall(psycheGrav, psycheRb, psycheStart);
    }

    public void OnButtonRelease()
    {
        isPressed = false;
        button.transform.position = unpressed.position;
    }

    private void ResetBall(GameObject ball, Rigidbody rb, Transform start)
    {
        ball.transform.position = start.position;
        ball.transform.rotation = start.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
