using Unity.Netcode;
using UnityEngine;

public class Knife : MonoBehaviour
{

    public Rigidbody rb;
    public bool dangerous;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dangerous) {
            dangerous = false;
        }
    }

    public void Stuck() {
        dangerous = true;
        GameObject.Find("GameController").GetComponent<NetworkControl>().IncreaseDestruction();
        //rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Unstuck()
    {
        //rb.constraints = RigidbodyConstraints.None;
    }

    [ServerRpc(RequireOwnership = false)]
    public void KnifeDangerousServerRpc()
    {
        GameObject.Find("GameController").GetComponent<NetworkControl>().IncreaseDestruction();
    }
}
