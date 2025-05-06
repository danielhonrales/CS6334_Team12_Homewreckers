using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Netcode;

public class GazeInteractor : NetworkBehaviour 
{

    public ControllerInteractor controllerInteractor;
    public GameObject cameraObject;
    public GameObject gazedObject;
    public GameObject gazedUI;
    public GameObject reticle;
    public LineRenderer lineRenderer;
    public Vector3 raycastOffset;
    public Teleportation teleportation;
    public int raycastLength;
    public int raycastLengthBeforeUI;
    public GameObject avatar;
    public GameObject player;

    public LayerMask layerMask;

    private float previousPitch;
    private float previousDelta;

    public NetworkVariable<float> yRot = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previousPitch = cameraObject.transform.eulerAngles.x;
        if (IsOwner) {
            avatar.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        avatar.transform.rotation = Quaternion.Euler(avatar.transform.rotation.eulerAngles.x, yRot.Value, avatar.transform.rotation.eulerAngles.z);
        if (IsOwner) {
            SetYRotServerRpc(cameraObject.transform.rotation.eulerAngles.y);
        }
        if (IsOwner) {
            //player.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, cameraObject.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
            //reticle.SetActive(true);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, cameraObject.transform.position + (cameraObject.transform.up * -0.1f) + (cameraObject.transform.right * 0.1f));
            Outline outline;

            Ray ray = new(cameraObject.transform.position + raycastOffset, cameraObject.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, layerMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                //Debug.Log(hitObject.name);

                if (hitObject.layer == LayerMask.NameToLayer("Interactable")) {
                    if (hitObject != gazedObject)
                    {
                        if (gazedObject != null && (outline = gazedObject.GetComponent<Outline>()) != null) {
                            outline.enabled = false;
                        }
                        // Update current gazed object
                        gazedObject = hitObject;
                        if ((outline = gazedObject.GetComponent<Outline>()) != null)
                        {
                            outline.enabled = true;
                        }
                    }
                    lineRenderer.SetPosition(1, hit.point);
                } else {
                    if (gazedObject != null && (outline = gazedObject.GetComponent<Outline>()) != null) {
                        outline.enabled = false;
                    }
                    gazedObject = null;
                }

                if (hitObject.layer == LayerMask.NameToLayer("Door")) {
                    if (hitObject != gazedObject)
                    {
                        if (gazedObject != null && (outline = gazedObject.GetComponent<Outline>()) != null)
                        {
                            outline.enabled = false;
                        }
                        // Update current gazed object
                        gazedObject = hitObject;
                        if ((outline = gazedObject.GetComponent<Outline>()) != null)
                        {
                            outline.enabled = true;
                        }
                    }
                    lineRenderer.SetPosition(1, hit.point);
                    teleportation.ChangeRooms();
                } else {
                    teleportation.NoTerrainHit();
                }

                /* if (hitObject.layer == LayerMask.NameToLayer("UI")) {
                    if (gazedUI != null && (button = gazedUI.GetComponent<ObjectMenuButton>()) != null) {
                        button.hovered = false;
                    }
                    // Update current gazed object
                    gazedUI = hitObject;
                    if ((button = gazedUI.GetComponent<ObjectMenuButton>()) != null) {
                        button.hovered = true;
                    }

                    lineRenderer.SetPosition(1, hit.point);
                } else {
                    if (gazedUI != null && (button = gazedUI.GetComponent<ObjectMenuButton>()) != null) {
                        button.hovered = false;
                    }
                    gazedUI = null;
                } */
            } else {
                if (gazedObject != null && (outline = gazedObject.GetComponent<Outline>()) != null) {
                    outline.enabled = false;
                }
                gazedObject = null;
                teleportation.NoTerrainHit();
                /* if (gazedUI != null && (button = gazedUI.GetComponent<ObjectMenuButton>()) != null) {
                    button.hovered = false;
                } */
                gazedUI = null;
                lineRenderer.SetPosition(1, cameraObject.transform.position + (cameraObject.transform.forward * raycastLength));
            }

            float currentPitch = cameraObject.transform.eulerAngles.x;
            float deltaPitch = Mathf.DeltaAngle(previousPitch, currentPitch);

            if (Mathf.Abs(deltaPitch) > .5f && previousDelta != Math.Sign(deltaPitch)) // tune this threshold
            {
                if (controllerInteractor.grabbedObject != null && controllerInteractor.grabbedObject.name == "Guitar") {
                    controllerInteractor.grabbedObject.GetComponent<InteractableObject>().TriggerInteraction();
                }
            }
            previousPitch = currentPitch;
            previousDelta = Math.Sign(deltaPitch);
        }
    }

    public void RaycastForUI() {
        raycastLengthBeforeUI = raycastLength;
        raycastLength = 9999;
    }

    public void ResetRaycastLength() {
        raycastLength = raycastLengthBeforeUI;
        controllerInteractor.openObjectMenu = null;
    }

    void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    void OnEnable()
    {
        lineRenderer.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetYRotServerRpc(float value)
    {
        yRot.Value = value;
    }
}
