using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TerrainEngine
{
    /// <summary>
    /// Class that sends RPCs from pins between all players in a multiplayer room
    /// </summary>
    public class PinRPCS : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)] // anyone can send this RPC
        public void PinServerRpc(Vector3 position, string data, string guid, ServerRpcParams serverRpcParams = default)
        {
            Debug.Log("received rpc data" + data);
            //Guest Pin
            if (IsHost) 
            {
                // Spawn pin from host's end
                PerPixelDataReader.singleton.SpawnPin(position, data, guid);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemovePinServerRpc(string guid, ServerRpcParams serverRpcParams = default)
        {
            print("received request to delete pins" + guid);
            //Guest Pin
            if (IsHost)
            {
                // Remove pin from host's end
                PerPixelDataReader.singleton.RemovePinsWithGuid(guid);
            }
        }
    }
}