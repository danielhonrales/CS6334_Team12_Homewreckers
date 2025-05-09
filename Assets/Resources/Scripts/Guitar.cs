using Unity.Netcode;
using UnityEngine;

public class Guitar : NetworkBehaviour
{

    public Rigidbody rb;
    public AudioSource musicSource;
    public AudioSource audioSource;
    public Mesh mesh;
    public MeshFilter meshFilter;

    public int score;

    public NetworkVariable<bool> interactedWith = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> broken = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (broken.Value && meshFilter.mesh != mesh) {
            meshFilter.mesh = mesh;
        }
        if (transform.position.y < -.3f)
        {
            transform.position = new Vector3(transform.position.x, .1f, transform.position.z);
        }
    }

    public void Grab() {
        rb.constraints = RigidbodyConstraints.None;
        SetInteractedWithServerRpc(true);
    }

    public void Release() {
        if (!IsOwner) return;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(GameObject.Find("PlayerMe").GetComponent<GazeInteractor>().cameraObject.transform.forward * 1000f);
        
        audioSource.Play();
    }

    public void Interact() {
        if (!broken.Value) {
            Debug.Log("play guitar");
            musicSource.pitch = Random.Range(.5f, 2f);
            musicSource.Play();

            score++;
            if (score >= 3) {
                GameObject.Find("GameController").GetComponent<NetworkControl>().DecreaseDestructionServerRpc();
                score = 0;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (!GameObject.Find("PlayerMe").GetComponent<CharacterMovement>().adult.Value) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && interactedWith.Value && !broken.Value) {
                meshFilter.mesh = mesh;
                GameObject.Find("GameController").GetComponent<NetworkControl>().IncreaseDestructionServerRpc();
                SetBrokenServerRpc(true);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetInteractedWithServerRpc(bool value)
    {
        interactedWith.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetBrokenServerRpc(bool value)
    {
        broken.Value = value;
    }
}
