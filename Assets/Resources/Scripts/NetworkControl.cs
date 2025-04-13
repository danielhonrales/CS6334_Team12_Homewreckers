using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.Networking.PlayerConnection;
using System;

public class NetworkControl : MonoBehaviour
{

    public NetworkVariable<int> destructionPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void StartServer() {
        NetworkManager.Singleton.StartServer();
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost() {
        NetworkManager.Singleton.StartHost();
    }

    public void Update()
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost) {
            if (Input.GetButtonDown(ButtonMappings.GetMapping("Menu")) || Input.GetKeyDown(KeyCode.M)) {
                ConnectPlayer();
            }
        }
    }

    public async void ConnectPlayer() {
        Debug.Log("Starting client");
        StartClient();
        await Task.Delay(1000);
        
        if (!NetworkManager.Singleton.IsConnectedClient) {
            Debug.Log("No host found, Starting host");
            NetworkManager.Singleton.Shutdown();
            await Task.Yield();
            StartHost();
        } else {
            Debug.Log("Connected as client");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseDestruction()
    {
        destructionPoints.Value += 1;
        destructionPoints.Value = Math.Min(destructionPoints.Value, 4);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecreaseDestruction()
    {
        destructionPoints.Value -= 1;
        destructionPoints.Value = Math.Max(destructionPoints.Value, 0);
    }
    
}
