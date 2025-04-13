using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : NetworkBehaviour
{

    public List<MainMenuButton> menuButtons = new();
    public int currentButton;
    public bool interactable = true;

    public CharacterMovement characterMovement;
    public ControllerInteractor controllerInteractor;
    public GazeInteractor gazeInteractor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (IsOwner) {
            if (interactable) {
                int verticalJs = (Input.GetAxis("Vertical") == 0) ? 0 : Math.Sign(Input.GetAxis("Vertical") * -1);

                if (verticalJs != 0) {
                    StartCoroutine(CooldownMenu());
                    SetButton((currentButton + verticalJs) % menuButtons.Count);
                }

                if (Input.GetButtonDown(ButtonMappings.GetMapping("Trigger")) || Input.GetKeyDown(KeyCode.Q)) {
                    StartCoroutine(CooldownMenu());
                    menuButtons[currentButton].OnClick();
                }

                if (Input.GetButtonDown(ButtonMappings.GetMapping("Menu")) || Input.GetKeyDown(KeyCode.Q)) {
                    CloseMenu();
                }
            }
        //}
    }

    public void SetButton(int newButton) {
        if (newButton < 0) {
            newButton = menuButtons.Count + newButton;
        }
        menuButtons[currentButton].GetComponent<Image>().color = Color.white;
        currentButton = newButton;
        menuButtons[currentButton].GetComponent<Image>().color = Color.yellow;
    }

    public void CloseMenu() {
        characterMovement.enabled = true;
        controllerInteractor.enabled = true;
        gazeInteractor.enabled = true;
        gameObject.SetActive(false);
    }

    public IEnumerator CooldownMenu() {
        interactable = false;
        yield return new WaitForSeconds(0.25f);
        interactable = true;
    }

    void OnEnable()
    {
        interactable = true;
    }
}
