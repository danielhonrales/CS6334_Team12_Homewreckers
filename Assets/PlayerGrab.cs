using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public GameObject guitar;
    
    public GameObject playerHand;
    public float handPower;
    bool inHands=false;
    Vector3 guitarpos;
    Collider guitarCol;
    Rigidbody guitarRb;
    Camera cam;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // guitarpos= guitar.transform.position;
        guitarCol=guitar.GetComponent<SphereCollider>();
        guitarRb=guitar.GetComponent<Rigidbody>();
        cam=GetComponentInChildren<Camera>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!inHands && playerHand.GetComponent<Grab>().canGrab)
            {
                guitarCol.isTrigger=true;
                guitar.transform.SetParent(playerHand.transform);
                guitar.transform.localPosition=playerHand.transform.localPosition;
                // Note: guitar.transform.localPosition= new Vector3(nearest co-ordinates of multiplayer);
                guitarRb.linearVelocity=Vector3.zero;
                guitarRb.useGravity=false;
                inHands=true;

            } else if(inHands)
            {
                guitarCol.isTrigger=false;
                guitarRb.useGravity=true;
                this.GetComponent<PlayerGrab>().enabled=false;
                guitar.transform.SetParent(null);
                guitar.transform.localPosition= guitarpos;
                guitarRb.linearVelocity = cam.transform.rotation * Vector3.forward * handPower;
                inHands=false;
            }
            
        }
        
    }
}
