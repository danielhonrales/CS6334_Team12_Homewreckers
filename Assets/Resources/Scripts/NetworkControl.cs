using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using System;
using Unity.Netcode.Transports.UTP;
using TMPro;
using Unity.VisualScripting;

public class NetworkControl : NetworkBehaviour
{

    public NetworkVariable<int> destructionPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public TMP_InputField inputIP;
    public GameObject startCamera;

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
        if (inputIP.text == "-") {
            var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            inputIP.text = transport.ConnectionData.Address;
        }
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost) {
            if (Input.GetButtonDown(ButtonMappings.GetMapping("Menu")) || Input.GetKeyDown(KeyCode.M)) {
                ConnectPlayer();
            }
        }
    }

    public async void ConnectPlayer() {
        Debug.Log("Starting client");
        var clientTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        clientTransport.ConnectionData.Address = inputIP.text;
        //clientTransport.ConnectionData.Port = 7777;
        StartClient();
        await Task.Delay(1000);
        
        if (!NetworkManager.Singleton.IsConnectedClient) {
            Debug.Log("No host found, Starting host");
            NetworkManager.Singleton.Shutdown();
            await Task.Yield();
            var hostTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            hostTransport.ConnectionData.Address = inputIP.text;
            //hostTransport.ConnectionData.Port = 7777;
            StartHost();
        } else {
            Debug.Log("Connected as client");
        }

        inputIP.gameObject.SetActive(false);
        startCamera.SetActive(false);
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
