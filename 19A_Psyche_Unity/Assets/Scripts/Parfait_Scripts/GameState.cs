using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Netcode;
//using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    enum BuildType // Platforms we allow
    {
        Desktop,
        VR
    }

    public static bool InMultiuser { get; set; } // determines if the user is in a Multiplayer session
    public static bool InTerrain { get; set; } // determines if the user is in a Terrain
    public static bool IsVR = true; // determines if the user is in VR

    [SerializeField] private BuildType SelectBuildType = BuildType.Desktop; // determines if we should make a desktop or VR build

    public static bool generateDataImages;
    public static bool printPerPixelCoordinates;
    public static bool vrDebug; // vrDebug allows the developer to view the menus in world-space as a desktop user

    // Desktop / VR rig variabes
    [SerializeField] private GameObject xrRig;
    //[SerializeField] private GameObject armature;
    //[SerializeField] private Canvas mainCanvas;
    //[SerializeField] private GameObject xrRigPrefab;
    //[SerializeField] private GameObject desktopPrefab;

    void Awake()
    {
        SetBuildType();
        
        // Set up player rig
        if (IsVR)
        {
            SetUpMeshVR();
        }
        else
        {
            SetUpMeshDesktop();
        }
    }

    void Start()
    {
        InMultiuser = false;
    }

    private void SetBuildType()
    {
        switch (SelectBuildType)
        {
            case BuildType.VR:
                IsVR = true;
                //mainCanvas.renderMode = RenderMode.WorldSpace;
                print("VR build");
                break;
            case BuildType.Desktop:
                IsVR = false;
                //mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                //mainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
                print("Desktop build");
                break;
            default:
                Debug.LogError("Unable to find build target. Please try again");
                break;
        }
    }
    
    private void SetUpMeshDesktop()
    {
        // armature.SetActive(true);
        xrRig.SetActive(false);
    }

    private void SetUpMeshVR()
    {
        // armature.SetActive(false);
        xrRig.SetActive(true);
    }
}
