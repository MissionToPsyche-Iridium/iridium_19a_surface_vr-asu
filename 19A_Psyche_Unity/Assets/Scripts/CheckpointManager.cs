using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;

    private ParticleSystem particles;


    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if(score >= 7)
        {
            particles.Play();
        }
        scoreText.text = "Current Score: " + score;
    }
}
