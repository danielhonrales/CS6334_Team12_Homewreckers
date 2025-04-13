using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{

    [SerializeField] private UnityEvent OnGrabEvent;
    [SerializeField] private UnityEvent OnInteractEvent;
    public bool grabbable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerGrab() {
        OnGrabEvent?.Invoke();
    }

    public void TriggerInteraction() {
        OnInteractEvent?.Invoke();
    }
}
