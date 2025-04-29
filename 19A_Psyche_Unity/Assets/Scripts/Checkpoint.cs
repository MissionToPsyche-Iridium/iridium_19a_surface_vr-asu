using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameObject manager;
    public bool hasTriggered = false;

    private ParticleSystem particles;


    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("CheckpointManager");
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Rover") && !hasTriggered)
        {
            manager.GetComponent<CheckpointManager>().score++;
            hasTriggered = true;
            particles.Play();
            manager.GetComponent<CheckpointManager>().Celebration();
        }
    }

}
