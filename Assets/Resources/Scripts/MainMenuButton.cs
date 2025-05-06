using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuButton : MonoBehaviour
{

    [SerializeField] private UnityEvent OnClickEvent;
    public MainMenu mainMenu;
    public TMP_Text buttonText;
    public CharacterMovement characterMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick() {
        OnClickEvent.Invoke();
    }

    public void Resume() {
        mainMenu.CloseMenu();
    }

    public void Swap() {
        characterMovement.ToggleRole();
        mainMenu.CloseMenu();
    }

    public void StartRound() {
        GameObject.Find("GameController").GetComponent<GameController>().StartGame();
    }

    public void Quit() {
        Application.Quit();
    }
}
