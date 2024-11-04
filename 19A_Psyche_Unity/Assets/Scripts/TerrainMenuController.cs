using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject terrainMenu;
    private UnityEngine.XR.InputDevice rightController;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(!rightController.isValid)
        {
            rightController = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        }

        bool value;
        if(rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out value) && value)
        {
            EnableMenu();
        }
    }

    private void EnableMenu()
    {
        terrainMenu.transform.position = cam.transform.position + cam.transform.forward * 1;
        terrainMenu.transform.LookAt(cam.transform);
        terrainMenu.transform.Rotate(new Vector3(0, 180, 0));
    }
}
