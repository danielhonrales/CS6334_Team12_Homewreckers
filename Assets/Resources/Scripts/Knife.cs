using Unity.Netcode;
using UnityEngine;

public class Knife : NetworkBehaviour
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
        float bias = 0.15f;
        Vector3 forward = GameObject.Find("PlayerMe").GetComponent<GazeInteractor>().cameraObject.transform.forward;
        Vector3 up = Vector3.up;
        Vector3 releaseDirection = Vector3.Slerp(forward, up, bias);
        Debug.Log(releaseDirection);
        rb.AddForce(releaseDirection * 30f, ForceMode.Impulse);
        
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
