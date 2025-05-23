using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WireShock : MonoBehaviour
{
    // Reference to the destruction bar value
    public float destructionBar = 0f;
    //public float shockDamage = 10f; // Amount to add to the destruction bar on shock
    public float interactionRadius = 1f; // Radius within which the player can interact
    public bool interactable = true;
    public AudioSource audioSource;


    private void Update()
    {

    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (!other.gameObject.GetComponent<CharacterMovement>().adult.Value) {
            if (interactable) {
                StartCoroutine(Cooldown());
                OnShock(); // Trigger shock effect
                ShockServerRpc();
                audioSource.Play();
                Debug.Log("Player interacted with wire! Destruction bar increased.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize interaction radius in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    // Open function for handling shock effects (customize this as needed)
    public void OnShock()
    {
        // Example: Play sound, visual effect, or reduce player health
        Debug.Log("Player shocked!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShockServerRpc()
    {
        GameObject.Find("GameController").GetComponent<NetworkControl>().IncreaseDestructionServerRpc();
    }

    public IEnumerator Cooldown() {
        interactable = false;
        yield return new WaitForSeconds(5f);
        interactable = true;
    }
}
