using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{

    public int initialTime;
    public NetworkVariable<int> time = new(
        60, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> roundActive = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Countdown() {
        while (time.Value >= 0) {
            yield return new WaitForSeconds(1);
            SetTimeServerRpc(time.Value - 1);
        }
        SetTimeServerRpc(0);
        SetRoundActiveServerRpc(false);
        EndGame();
    }

    public void StartGame() {
        SetTimeServerRpc(initialTime);
        SetRoundActiveServerRpc(true);
        StartCoroutine(Countdown());
    }

    public void EndGame() {

    }

    [ServerRpc(RequireOwnership = false)]
    void SetTimeServerRpc(int value)
    {
        time.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetRoundActiveServerRpc(bool value)
    {
        roundActive.Value = value;
    }
}
