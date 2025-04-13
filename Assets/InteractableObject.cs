using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{

    [SerializeField] private UnityEvent OnClickEvent;
    public bool grabbable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerInteraction() {
        OnClickEvent.Invoke();
    }
}
