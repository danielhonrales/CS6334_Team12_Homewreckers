using UnityEngine;
using Unity.Netcode;

public class Stove : NetworkBehaviour
{

    public Outline outline;
    public NetworkVariable<bool> isOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public ParticleSystem fire;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn.Value && !fire.isPlaying) {
            fire.Play();
        } 

        if (!isOn.Value && fire.isPlaying) {
            fire.Stop();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TurnOnServerRpc()
    {
        if (!isOn.Value) {
            isOn.Value = true;
            GameObject.Find("GameController").GetComponent<NetworkControl>().IncreaseDestruction();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TurnOffServerRpc()
    {
        if (isOn.Value) {
            isOn.Value = false;
            GameObject.Find("GameController").GetComponent<NetworkControl>().DecreaseDestruction();
        }
    }
}
