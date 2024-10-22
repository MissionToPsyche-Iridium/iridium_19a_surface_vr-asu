using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoverMover : MonoBehaviour
{
    [SerializeField] private float moveForce = 10f;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    public void OnForwardSelectStart()
    {
        rigidbody.AddForce(Vector3.forward * moveForce);
        Debug.Log("TEST");
    }
}
