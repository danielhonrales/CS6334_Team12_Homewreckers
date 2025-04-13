using UnityEngine;

public class WireShock : MonoBehaviour
{
    // Reference to the destruction bar value
    public float destructionBar = 0f;
    public float shockDamage = 10f; // Amount to add to the destruction bar on shock

    // Trigger detection for wire collision
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding is the player
        if (other.CompareTag("Player"))
        {
            // Call the OnShock function
            OnShock();

            // Add shock damage to the destruction bar
            destructionBar += shockDamage;

            // Optional: Debug message for testing
            Debug.Log("Player touched the wire! Destruction bar increased.");
        }
    }

    // Open function for handling shock effects (customize this as needed)
    public void OnShock()
    {
        // Example: Play sound, visual effect, or reduce player health
        Debug.Log("Player shocked!");
    }
}
