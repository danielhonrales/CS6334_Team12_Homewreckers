using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;

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

    public Animator animator;
    public NetworkVariable<bool> grabAnim = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> throwAnim = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
        if (grabAnim.Value) {
            SetGrabAnimServerRpc(false);
            animator.Play("Grab");
        }

        if (throwAnim.Value) {
            SetThrowAnimServerRpc(false);
            animator.Play("Throw");
        }

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
                        grabbedObject.transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 30f, Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 30f, 0f);
                    }/*  else { 
                        Vector3 targetPosition = gazeInteractor.cameraObject.transform.position + gazeInteractor.cameraObject.transform.forward * grabOffset.z + gazeInteractor.cameraObject.transform.right * grabOffset.x + gazeInteractor.cameraObject.transform.up * grabOffset.y;
                        Quaternion targetRotation = Quaternion.Euler(Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 45f, Mathf.Sin(Time.time * 60f * Mathf.Deg2Rad) * 45f, 0f);
                        RequestMoveClientRpc(targetPosition, targetRotation);
                    } */
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetGrabAnimServerRpc(bool value)
    {
        grabAnim.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetThrowAnimServerRpc(bool value)
    {
        throwAnim.Value = value;
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
            SetGrabAnimServerRpc(true);
            gazedObject.layer = LayerMask.NameToLayer("Grabbed");
            gazedObject.GetComponent<Collider>().enabled = false;
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
            Debug.Log(requestingClientId + "claiming ownership of knife");
            networkObject.ChangeOwnership(requestingClientId);
        }
    }
    

    public void UngrabObject() {
        if (grabbedObject != null) {
            SetThrowAnimServerRpc(true);
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.transform.parent = null;
            grabbedObject.GetComponent<Collider>().enabled = true;
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            grabbedObject.GetComponent<InteractableObject>().grabbable = true;
            grabbedObject.GetComponent<InteractableObject>().TriggerRelease();
            StartCoroutine(ReturnToServer());
            grabbedObject = null;
        }
    }

    private IEnumerator ReturnToServer() {
        yield return new WaitForSeconds(1);
        ReturnOwnershipToServerRpc(grabbedObject.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    void ReturnOwnershipToServerRpc(NetworkObjectReference networkObjectRef)
    {
        if (networkObjectRef.TryGet(out NetworkObject networkObject)) {
            networkObject.ChangeOwnership(NetworkManager.ServerClientId);
        }
    }
}
