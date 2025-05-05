using Unity.Netcode;
using UnityEngine;

public class ChildPlayer : NetworkBehaviour
{
    public GameObject vrCamera;
    public GameObject inputRig;

    void Start()
    {
        if (!IsOwner)
        {
            // Disable camera and input for remote players
            if (vrCamera != null)
                vrCamera.SetActive(false);
            if (inputRig != null)
                inputRig.SetActive(false);
        }
        else
        {
            Debug.Log("🎮 Local Child player setup complete!");
        }
    }
}