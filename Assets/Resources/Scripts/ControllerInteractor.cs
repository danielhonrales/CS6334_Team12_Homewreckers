using System;
using UnityEngine;
using Unity.Netcode;

public class ControllerInteractor : NetworkBehaviour
{

    public GazeInteractor gazeInteractor;
    public CharacterMovement characterMovement;

    public float translateSpeed;
    public float rotateSpeed;

    public GameObject openObjectMenu;
    public GameObject mainMenu;
    public GameObject grabbedObject;
    public Vector3 grabOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        mainMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner) {
            /* for (int i = 0; i < 15; i++) {
                if (Input.GetAxis(string.Format("js{0}", i)) != 0) {
                    Debug.Log(string.Format("js{0}", i));
                }
            } */

            if (mainMenu.activeSelf != true) {
                /* // Object Menu
                if (Input.GetButton("js2") || Input.GetKeyDown(KeyCode.Q)) {
                    GameObject gazedObject = gazeInteractor.gazedObject;
                    if (gazedObject != null && gazedObject.transform.Find("ObjectMenu") != null) {
                        if (openObjectMenu != null && openObjectMenu != gazedObject.transform.Find("ObjectMenu").gameObject) {
                            openObjectMenu.SetActive(false);
                        }
                        openObjectMenu = gazedObject.transform.Find("ObjectMenu").gameObject;
                        openObjectMenu.SetActive(true);
                        characterMovement.enabled = false;
                        gazeInteractor.RaycastForUI();
                    }
                } */

                if (Input.GetButtonDown(ButtonMappings.GetMapping("Trigger")) || Input.GetKeyDown(KeyCode.Q))
                {
                    if (grabbedObject == null) {
                        GameObject gazedObject = gazeInteractor.gazedObject;
                        if (gazedObject) {
                            InteractableObject interactableObject = gazedObject.GetComponent<InteractableObject>();
                            if (interactableObject && interactableObject.grabbable) {
                                GrabObject(gazedObject);
                                interactableObject.TriggerGrab();
                            }
                        }
                    } else {
                        UngrabObject();
                    }
                }

                if (Input.GetButtonDown(ButtonMappings.GetMapping("A")) || Input.GetKeyDown(KeyCode.A))
                {
                    GameObject gazedObject = gazeInteractor.gazedObject;
                    if (gazedObject) {
                        InteractableObject interactableObject = gazedObject.GetComponent<InteractableObject>();
                        if (interactableObject) {
                            interactableObject.TriggerGrab();
                        }
                    }

                    if (grabbedObject) {
                        InteractableObject interactableObject = grabbedObject.GetComponent<InteractableObject>();
                        if (interactableObject) {
                            interactableObject.TriggerInteraction();
                        }
                    }
                }

                // Main Menu
                if (Input.GetButtonDown(ButtonMappings.GetMapping("Menu")) || Input.GetKeyDown(KeyCode.M)) {
                    characterMovement.enabled = false;
                    gazeInteractor.enabled = false;
                    if (openObjectMenu != null) {
                        openObjectMenu.SetActive(false);
                        openObjectMenu = null;
                    }
                    this.enabled = false;
                    mainMenu.SetActive(true);
                }

                // Grabbed Object tracking
                if (grabbedObject != null) {
                    if (grabbedObject.GetComponent<NetworkObject>().IsOwner) {
                        grabbedObject.transform.position = gazeInteractor.cameraObject.transform.position + gazeInteractor.cameraObject.transform.forward * grabOffset.z + gazeInteractor.cameraObject.transform.right * grabOffset.x + gazeInteractor.cameraObject.transform.up * grabOffset.y;
                        //grabbedObject.transform.localPosition = new Vector3(grabbedObject.transform.localPosition.x, Math.Min(0.5f, grabbedObject.transform.localPosition.y), grabbedObject.transform.localPosition.z);
                        grabbedObject.transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 45f, Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 45f, 0f);
                    }/*  else { 
                        Vector3 targetPosition = gazeInteractor.cameraObject.transform.position + gazeInteractor.cameraObject.transform.forward * grabOffset.z + gazeInteractor.cameraObject.transform.right * grabOffset.x + gazeInteractor.cameraObject.transform.up * grabOffset.y;
                        Quaternion targetRotation = Quaternion.Euler(Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 45f, Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 45f, 0f);
                        RequestMoveClientRpc(targetPosition, targetRotation);
                    } */
                }
            }
        }
    }

    [ClientRpc(RequireOwnership = false)]
    void RequestMoveClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        Debug.Log(name + " is grabbing something");
        grabbedObject.transform.position = newPosition;
        grabbedObject.transform.localRotation = newRotation;
    }

    public void TranslateObject() {
        GameObject gazedObject = gazeInteractor.gazedObject;
        if (gazedObject != null) {
            gazedObject.GetComponent<Rigidbody>().MovePosition(gazedObject.transform.position + new Vector3(gazeInteractor.cameraObject.transform.forward.x, 0, gazeInteractor.cameraObject.transform.forward.z) * translateSpeed);
        }
    }

    public void RotateObject() {
        GameObject gazedObject = gazeInteractor.gazedObject;
        if (gazedObject != null) {
            gazedObject.transform.Rotate(new(0, rotateSpeed, 0));
        }
    }

    public void GrabObject(GameObject objectToGrab = null) {
        UngrabObject();
        GameObject gazedObject = (objectToGrab == null) ? gazeInteractor.gazedObject : objectToGrab;
        if (gazedObject != null) {
            gazedObject.layer = LayerMask.NameToLayer("Grabbed");
            gazedObject.GetComponent<Collider>().enabled = false;
            gazedObject.GetComponent<Rigidbody>().freezeRotation = true;
            //gazedObject.transform.parent = gazeInteractor.cameraObject.transform;
            grabbedObject = gazedObject;
            RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId, grabbedObject.GetComponent<NetworkObject>());
            grabbedObject.GetComponent<InteractableObject>().grabbable = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestOwnershipServerRpc(ulong requestingClientId, NetworkObjectReference networkObjectRef)
    {
        if (networkObjectRef.TryGet(out NetworkObject networkObject)) {
            networkObject.ChangeOwnership(requestingClientId);
        }
    }
    

    public void UngrabObject() {
        if (grabbedObject != null) {
            grabbedObject.transform.parent = null;
            grabbedObject.GetComponent<Collider>().enabled = true;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.GetComponent<Rigidbody>().freezeRotation = false;
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            grabbedObject.GetComponent<InteractableObject>().grabbable = true;
            grabbedObject.GetComponent<InteractableObject>().TriggerRelease();
            ReturnOwnershipToServerRpc(grabbedObject.GetComponent<NetworkObject>());
            grabbedObject = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void ReturnOwnershipToServerRpc(NetworkObjectReference networkObjectRef)
    {
        if (networkObjectRef.TryGet(out NetworkObject networkObject)) {
            networkObject.ChangeOwnership(NetworkManager.ServerClientId);
        }
    }
}
