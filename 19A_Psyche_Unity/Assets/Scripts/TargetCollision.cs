using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetCollision : MonoBehaviour
{
    public TextMeshPro textMesh; // Assign this in the Inspector

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "EarthBall":
                textMesh.text = "Our home planet, Earth, has a gravitational constant of 9.81 Meters/Second^2";
                break;
            case "MarsBall":
                textMesh.text = "Mars Hit";
                break;
            case "PsycheBall":
                textMesh.text = "Psyche Hit";
                break;
            default:
                textMesh.text = "Unexpected Collision";
                break;
        }
    }
}