using UnityEngine;

public class Grab : MonoBehaviour
{
    public bool canGrab=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Guitar"))
        {
            Debug.Log("I can Grab");
            canGrab=true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("I can't Grab");
        canGrab=false;
    }
}
