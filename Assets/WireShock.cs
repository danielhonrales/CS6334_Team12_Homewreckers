using UnityEngine;

public class WireShock : MonoBehaviour
{
    // Reference to the destruction bar value
    public float destructionBar = 0f;
    public float shockDamage = 10f; // Amount to add to the destruction bar on shock
    public float interactionRadius = 3f; // Radius within which the player can interact
    public Transform player; // Reference to the player's transform

    private bool isPlayerInRange = false; // Tracks if the player is in range

    private void Update()
    {
        // Check if the player is within interaction radius
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distanceToPlayer <= interactionRadius;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // Press 'E' to interact
        {
            OnShock(); // Trigger shock effect
            destructionBar += shockDamage; // Add shock damage to destruction bar
            Debug.Log("Player interacted with wire! Destruction bar increased.");
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
}
