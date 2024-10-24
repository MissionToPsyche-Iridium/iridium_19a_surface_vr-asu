using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using TerrainEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Networking;
using UserInterface;

namespace Multiuser
{

    /// <summary>
    /// On the pin prefab object. Synchronizes the pin number and values between host and client using NetworkVariables
    /// </summary>
    public class PerPixelSync : CustomUnnamedMessageHandler<string>
    {
        //using network variables to track theses values
        public NetworkVariable<FixedString32Bytes> pinNumber = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<FixedString512Bytes> pinData = new NetworkVariable<FixedString512Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        //public NetworkVariable<FixedString512Bytes> pinNotes = new NetworkVariable<FixedString512Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        
        private void Awake()
        {
            //pinNotes.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(FixedString512Bytes previousvalue, FixedString512Bytes newvalue)
        {
            //print("New value " + newvalue);
            this.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text =
                "Pin #" + pinNumber.Value;
            this.transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = pinData.Value.ToString();
            /*this.transform.GetChild(0).transform.GetChild(2).GetComponent<TMP_InputField>().text =
                pinNotes.Value.ToString();*/

        }

        public void InputFieldUpdate(TMP_InputField inputField)
        {
            //print("updated text = " + inputField.text);
            //pinNotes.Value = inputField.text;
        }

        /// <summary>
        /// Called when a network opens or when a new pin is spawned into server (EX: when host opens a server, client joins room)
        /// </summary>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(!NetworkManager.Singleton.IsHost)
            {
                //PerPixelDataReader.pinList.Add(this.transform.GetChild(0).GetComponent<Pin>());
                transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text =
                    "Pin #" + pinNumber.Value;
                transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = pinData.Value.ToString();
                //this.transform.GetChild(0).transform.GetChild(2).GetComponent<TMP_InputField>().text = pinNotes.Value.ToString();
            }
            
        }

        /// <summary>
        /// Called when client leaves a room
        /// </summary>
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            //print("Called OnNetworkDespawn");
            
            //removes all pins (synced & local) when host disconnects
            if (IsHost)
            {
                //Disabled 11/14/2023 due to breaking individual control of pins
                //PerPixelDataReader.singleton.RemoveOldPins();
            }
        }
        

    }

}