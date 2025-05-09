using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Extinguisher : NetworkBehaviour
{

    public ParticleSystem foam;
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -.3f) {
            transform.position = new Vector3(transform.position.x, .1f, transform.position.z);
        }
    }

    public void Extinguish() {
        if (!IsOwner) return;
        StartCoroutine(Foam());
        GameObject gazedObject;
        gazedObject = GameObject.Find("PlayerMe").GetComponent<GazeInteractor>().gazedObject;
        if (gazedObject && gazedObject.name == "Stove") {
            gazedObject.GetComponent<Stove>().TurnOffServerRpc();
        }
    }

    public IEnumerator Foam() {
        foam.Play();
        audioSource.Play();
        yield return new WaitForSeconds(2f);
        foam.Stop();
    }

    public void Release() {
        ReturnOwnershipToServerRpc(GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    void ReturnOwnershipToServerRpc(NetworkObjectReference networkObjectRef)
    {
        if (networkObjectRef.TryGet(out NetworkObject networkObject)) {
            networkObject.ChangeOwnership(NetworkManager.ServerClientId);
        }
    }
}
