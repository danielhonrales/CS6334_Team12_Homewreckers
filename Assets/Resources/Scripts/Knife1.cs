using Unity.Netcode;
using UnityEngine;

public class Knife1 : NetworkBehaviour
{

    public Rigidbody rb;
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Grab() {
        rb.constraints = RigidbodyConstraints.None;
    }

    public void Release() {
        if (!IsOwner) return;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(GameObject.Find("PlayerMe").GetComponent<GazeInteractor>().cameraObject.transform.forward * 1000f);
        rb.isKinematic = false;
        
        audioSource.Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            
            GameObject.Find("GameController").GetComponent<NetworkControl>().IncreaseDestructionServerRpc();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("CuttingBoard")) {
            GameObject.Find("GameController").GetComponent<NetworkControl>().DecreaseDestructionServerRpc();
        }
    }
}
