using System.Collections;
using System.Collections.Generic;
using TMPro;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int score = 0;
    public float timePassed = 0f;
    public float bestTime = 0f;
    public bool runTime = false;
    public TextMeshProUGUI scoreText;

    private ParticleSystem particles;


    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        scoreText.text = "Current Score: " + score + '\n' + '\n' + "Current time: " + timePassed.ToString("0.000") + '\n' + '\n' + "Best time: " + bestTime.ToString("0.000");

        if(runTime)
        {
            timePassed += Time.deltaTime;
        }    
    }

    public void Celebration()
    {
        if (score >= 7)
        {
            particles.Play();
            runTime = false;
            if (timePassed < bestTime || bestTime == 0f)
            {
                bestTime = timePassed;
            }
        }
    }
}
