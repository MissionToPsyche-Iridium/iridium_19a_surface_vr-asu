using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHoverHandler : MonoBehaviour
{
    private Canvas infoCanvas;
    private bool isOn = false;
    private float rstTime = 1;
    private float cooldown = -1;
    // Start is called before the first frame update
    void Start()
    {
        infoCanvas = GetComponentInChildren<Canvas>();
        if (!infoCanvas)
        {
            Debug.LogWarning("Canvas is NULL");
            return;
        }
        infoCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(cooldown == -1)
        {
            infoCanvas.enabled = isOn;
            cooldown = 0;
        }
        else if(cooldown <= rstTime)
        {
            cooldown += Time.deltaTime;
        }
        else
        {
            cooldown = -1;
        }
            
    }

    public void OnHoverEnter()
    {
        isOn = true;
    }

    public void OnHoverExit()
    {
        isOn = false;
    }
}
