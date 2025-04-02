using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControllerInteractor : MonoBehaviour
{

    public GazeInteractor gazeInteractor;
    public CharacterMovement characterMovement;

    public float translateSpeed;
    public float rotateSpeed;

    public GameObject openObjectMenu;
    public GameObject mainMenu;
    public GameObject grabbedObject;
    public Vector3 grabOffset;

    public GameObject world;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 15; i++) {
            if (Input.GetAxis(string.Format("js{0}", i)) != 0) {
                Debug.Log(string.Format("js{0}", i));
            }
        }

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

            if (Input.GetButton("js2") || Input.GetKeyDown(KeyCode.Q))
            {
                GameObject gazedObject = gazeInteractor.gazedObject;
                GrabObject(gazedObject);
            }

                // Ungrab
            if (Input.GetButton("js10") || Input.GetKeyDown(KeyCode.A)) {
                if (grabbedObject != null) {
                    UngrabObject();
                }
            }

            // Main Menu
            if (Input.GetButton("js0") || Input.GetKeyDown(KeyCode.M)) {
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
                grabbedObject.transform.localPosition = Vector3.Scale(gazeInteractor.cameraObject.transform.forward, grabOffset);
                grabbedObject.transform.localPosition = new Vector3(grabbedObject.transform.localPosition.x, Math.Min(0.5f, grabbedObject.transform.localPosition.y), grabbedObject.transform.localPosition.z);
                grabbedObject.transform.rotation = gazeInteractor.cameraObject.transform.rotation;
            }
        }
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
            gazedObject.GetComponent<Rigidbody>().isKinematic = true;
            gazedObject.GetComponent<Rigidbody>().freezeRotation = true;
            gazedObject.transform.parent = transform.Find("XRCardboardRig");
            grabbedObject = gazedObject;
        }
    }

    public void UngrabObject() {
        if (grabbedObject != null) {
            grabbedObject.transform.parent = world.transform;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.GetComponent<Rigidbody>().freezeRotation = false;
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            grabbedObject = null;
        }
    }
}
