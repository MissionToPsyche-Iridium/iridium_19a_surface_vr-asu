using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravManager : MonoBehaviour
{
    public Transform[] balls;
    private Vector3[] originalPositions;

    void Start()
    {
        originalPositions = new Vector3[balls.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            originalPositions[i] = balls[i].position;
        }
    }

    public void ResetBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            Rigidbody rb = balls[i].GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            balls[i].position = originalPositions[i];
        }
    }
}

