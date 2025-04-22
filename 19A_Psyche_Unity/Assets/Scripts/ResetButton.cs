using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ResetButton : MonoBehaviour
{
    public GravManager gravManager;
    private AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XRBaseInteractor>())
        {
            gravManager.ResetBalls();
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
