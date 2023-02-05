using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public enum MenuType {main, options}

    MenuType currentMenu = MenuType.main;

    [SerializeField]
    GameObject[] mainMenuButtons;
    TextMeshProUGUI[] mainMenuButtonText = new TextMeshProUGUI[4];
    
    [SerializeField]
    GameObject[] optionsButtons;
    TextMeshProUGUI[] optionsButtonText = new TextMeshProUGUI[10];

    int mainMenuIndex;
    int optionsIndex;

    Color selectColor = new Color(0.393f,0.643f,0.877f,1);

    void Start()
    {
        mainMenuIndex = mainMenuButtons.Length;
        optionsIndex = optionsButtons.Length;

        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtonText[i] = mainMenuButtons[i].GetComponent<TextMeshProUGUI>();
        }
        for (int i = 0; i < optionsButtons.Length; i++)
        {
            optionsButtonText[i] = optionsButtons[i].GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (currentMenu == MenuType.main)
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SelectButton(mainMenuIndex);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                NavigateMenuButtons(1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                NavigateMenuButtons(-1);
            }
        } 
        else if (currentMenu == MenuType.options)
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SelectButton(optionsIndex);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                NavigateOptionsButtons(1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                NavigateOptionsButtons(-1);
            }
        }
    }

    void NavigateMenuButtons(int i)
    {
        mainMenuButtonText[mainMenuIndex].color = new Color(1,1,1,1);

        if (i >= 0) {
            mainMenuIndex--;
        } else {
            mainMenuIndex++;
        }

        if (mainMenuIndex > mainMenuButtons.Length) mainMenuIndex = 0;
        if (mainMenuIndex < 0) mainMenuIndex = mainMenuButtons.Length;

        mainMenuButtonText[mainMenuIndex].color = selectColor;
    }
    
    void NavigateOptionsButtons(int i)
    {
        optionsButtonText[optionsIndex].color = new Color(1,1,1,1);

        if (i >= 0) {
            optionsIndex--;
        } else {
            optionsIndex++;
        }

        if (optionsIndex > optionsButtons.Length) optionsIndex = 0;
        if (optionsIndex < 0) optionsIndex = optionsButtons.Length;

        optionsButtonText[optionsIndex].color = selectColor;
    }

    void SelectButton(int i)
    {
        switch(i){
            case 0:
                //Start Game
                break;
            case 1:
                //Move to options menu
                break;
            case 2:
                //Show Credits
                break;
            case 3:
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
