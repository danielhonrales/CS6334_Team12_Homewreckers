using UnityEngine;

public class Knife : MonoBehaviour
{

    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Stuck() {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Unstuck()
    {
        rb.constraints = RigidbodyConstraints.None;
    }
}
