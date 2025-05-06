using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System.Collections;

public class NetworkPlayerData : NetworkBehaviour
{

    public GameObject vrCameraRig;
    public LineRenderer lineRenderer;
    public NetworkVariable<FixedString32Bytes> playerName = new(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public CharacterMovement characterMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize() {
        while (vrCameraRig == null || lineRenderer == null || characterMovement == null) {
            yield return new WaitForSeconds(0.1f);
        }
        if (IsOwner) {
            if (IsHost) {
                gameObject.name = "PlayerMe";
            } else {
                gameObject.name = "PlayerMe";
            }
            
            vrCameraRig.SetActive(true);
            if (IsHost) {
                SetPlayerNameServerRpc("Host");
            } else {
                SetPlayerNameServerRpc("Client");
            }
            
        } else {
            vrCameraRig.SetActive(false);
            lineRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerNameServerRpc(string newName)
    {
        playerName.Value = newName;
    }
}
