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
                textMesh.text = "Our home planet, Earth, has a gravitational constant of\n 9.81 Meters/Second^2\n\nWith a circumfrence of 24,901 miles, Earth is the only the 5th largest planet in our solar system.";
                break;
            case "MarsBall":
                textMesh.text = "Mars is much smaller than our home planet.\n\nWith a circumfrence of 13,263 miles, Mars has only about 28% of the Earth's surface area.\n\nMars experiences only about 38% of Earths gravity.";
                break;
            case "PsycheBall":
                textMesh.text = "Psyche is an asteroid which is much much smaller than either planet.\n\n Psyche, at its widest, is only about 173 miles across.\nIt's surface area is roughly comparable to size of France.\n\n The gravity Psyche experiences is much lower as well, roughly 14% of Earth's gravity.";
                break;
            default:
                textMesh.text = "Unexpected Collision";
                break;
        }
    }
}