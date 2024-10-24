using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using Unity.Netcode;
using UnityEngine.Serialization;
//using UserInterface;

public class Pin: MonoBehaviour
{
    [Header("Pin Objects")]
    public GameObject pin; //pin object
    public GameObject panel; //panel Prefab
    public TMP_Text pinNumber; //pin count
    public TMP_Text pinData; //pin data
    public string data = ""; //list of data
    
    [Header("Pin Location")]
    public Vector3 position;
    /*public float xPosition;
    public int zPosition;*/
    
    [Header("Multiuser")]
    public ulong clientID = 0; // client who placed pin
    public string guid; //Holds players unique guid for individual deletion
}
