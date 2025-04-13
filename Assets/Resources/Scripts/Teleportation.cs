using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teleportation : MonoBehaviour
{

    public string currentRoom;
    public Transform kitchenPoint;
    public Transform livingRoomPoint;
    public RaycastHit terrainHit;
    public CharacterController characterController;

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

    public void ChangeRooms() {
        if (Input.GetButtonDown(ButtonMappings.GetMapping("X")) || Input.GetKeyDown(KeyCode.Y)) {
            if (currentRoom == "Kitchen")
            {
                currentRoom = "LivingRoom";
                TeleportPlayer(GameObject.Find("LivingRoomPoint").transform.position);
            }
            else if (currentRoom == "LivingRoom")
            {
                currentRoom = "Kitchen";
                TeleportPlayer(GameObject.Find("KitchenPoint").transform.position);
            }
        }
    }

    public void OnTerrainHit(RaycastHit hit) {

        if (Input.GetButton("js3") || Input.GetKey(KeyCode.Y)) {
            if (canTeleport) {
                canTeleport = false;
                TeleportPlayer(hit.point);
                StartCoroutine(CooldownTeleport());
            }
        }
    }

    public void NoTerrainHit() {
        //cue.SetActive(false);
    }

    public void TeleportPlayer(Vector3 point) {
        characterController.enabled = false;
        transform.position = point;
        characterController.enabled = true;
    }

    IEnumerator CooldownTeleport() {
        canTeleport = false;
        yield return new WaitForSeconds(1f);
        canTeleport = true;
    }
}
