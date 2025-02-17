using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameObject manager;
    private bool hasTriggered = false;


    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("CheckpointManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Rover") && !hasTriggered)
        {
            manager.GetComponent<CheckpointManager>().score++;
            hasTriggered = true;
            Debug.Log(manager.GetComponent<CheckpointManager>().score);
            gameObject.SetActive(false);
        }
    }

}
