using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHoverHandler : MonoBehaviour
{
    private Canvas infoCanvas;
    private bool isOn = false;
    private float rstTime = 1;
    private float cooldown = -1;
    private FacePlayer facePlayer;
    // Start is called before the first frame update
    void Start()
    {
        //Initialization
        infoCanvas = GetComponentInChildren<Canvas>();
        if (!infoCanvas)
        {
            Debug.LogWarning("Canvas is NULL");
            return;
        }
        infoCanvas.enabled = false;
        facePlayer = infoCanvas.GetComponent<FacePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Slight delay to prevent flickering UI
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
        
        //Make UI face player when it is on
        if(isOn){
            facePlayer.Activate();
        }
    }


    //Events that are called when player hovers or stops hovering over this item
    public void OnHoverEnter()
    {
        isOn = true;
    }

    public void OnHoverExit()
    {
        isOn = false;
    }
}
