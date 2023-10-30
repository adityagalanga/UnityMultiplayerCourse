using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{

    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRPC(ServerRpcParams serverRPCParams = default)
    {
        playerReadyDictionary[serverRPCParams.Receive.SenderClientId] = true;
        bool allClientReady = true;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientID) || !playerReadyDictionary[clientID])
            {
                allClientReady = false;
                break;
            }
        }
        if (allClientReady)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
}
