using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teleportation : MonoBehaviour
{

    public GameObject[] everythingElse;
    public GameObject cameraObject;
    public GameObject cue;
    public float maxDistance;
    public Vector3 raycastOffset;
    public RaycastHit terrainHit;

    private bool canTeleport;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canTeleport = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTerrainHit(RaycastHit hit) {
        cue.SetActive(true);
        cue.transform.position = hit.point;

        if (Input.GetButton("js3") || Input.GetKey(KeyCode.Y)) {
            if (canTeleport) {
                canTeleport = false;
                TeleportPlayer(hit.point);
                StartCoroutine(CooldownTeleport());
            }
        }
    }

    public void NoTerrainHit() {
        cue.SetActive(false);
    }

    public void TeleportPlayer(Vector3 point) {
        //transform.position = point + new Vector3(0, 1f, 0);
        Debug.Log(string.Format("tp to {0}", point + new Vector3(0, -1f, 0)));
        Vector3 diff = (point + new Vector3(0, 1f, 0)) - transform.position;
        foreach(GameObject obj in everythingElse) {
            obj.transform.position = new Vector3(obj.transform.position.x - diff.x, obj.transform.position.y - diff.y, obj.transform.position.z - diff.z);
        }
    }

    IEnumerator CooldownTeleport() {
        canTeleport = false;
        yield return new WaitForSeconds(1f);
        canTeleport = true;
    }
}
