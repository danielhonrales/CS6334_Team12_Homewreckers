using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using System;
using Unity.Netcode.Transports.UTP;

public class NetworkControl : NetworkBehaviour
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

    public override void OnNetworkSpawn() {
        
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
        var clientTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        //clientTransport.ConnectionData.Address = "192.168.0.196";
        //clientTransport.ConnectionData.Port = 7777;
        StartClient();
        await Task.Delay(1000);
        
        if (!NetworkManager.Singleton.IsConnectedClient) {
            Debug.Log("No host found, Starting host");
            NetworkManager.Singleton.Shutdown();
            await Task.Yield();
            var hostTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            //hostTransport.ConnectionData.Address = "0.0.0.0";
            //hostTransport.ConnectionData.Port = 7777;
            StartHost();
        } else {
            Debug.Log("Connected as client");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseDestructionServerRpc()
    {
        destructionPoints.Value += 1;
        destructionPoints.Value = Math.Min(destructionPoints.Value, 4);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecreaseDestructionServerRpc()
    {
        destructionPoints.Value -= 1;
        destructionPoints.Value = Math.Max(destructionPoints.Value, 0);
    }
    
}
