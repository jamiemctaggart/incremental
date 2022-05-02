using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GUI : MonoBehaviour
{
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI traditionText;
    public TextMeshProUGUI CurrentActionText;
    // Stability progress bar items
    public Slider progressBar;
    public TextMeshProUGUI stabilityText;
    public TextMeshProUGUI stabilityDropText;
    public TextMeshProUGUI timerText;
    // Building Timer progress bar items
    public Slider BuildingTimerProgressBar;
    public TextMeshProUGUI BuildingNameText;
    public TextMeshProUGUI BuildTimeText;

    // Buttons
    public GameObject FoodFocusButton;
    public GameObject UnrestFocusButton;

    // Death GUI section, starting with panel to cover other stuff
    public GameObject Panel;
    public TextMeshProUGUI LastedText;
    public TextMeshProUGUI StabilityIncrease;
    public GameObject DeathScreenGameObject;


    // Start is called before the first frame update
    public void Start()
    {
        FoodFocusButton.SetActive(false);
        UnrestFocusButton.SetActive(false);
        DisableDeathScreen();
    }

    public void TickUpdate(GameData gameData)
    {
        foodText.text = "Food: " + Math.Floor(gameData.resources[0] * 100) / 100;
        populationText.text = "Population: " + Math.Floor(gameData.population);
        //Progress bar stuff, value updates and the text to show the 3 values
        StabilityProgressBarUpdate(gameData);
        BuildingTimerProgressBarUpdate(gameData);
        traditionText.text = "Tradition: " + gameData.tradition;

    }

    public void DisableDeathScreen()
    {
        /*
        Panel.SetActive(false);
        DeathTextGameObject.SetActive(false);
        LastedTextGameObject.SetActive(false);
        StabilityIncreaseGameObject.SetActive(false);
        */
        DeathScreenGameObject.SetActive(false);
    }

    public void EnableDeathScreen()
    {
        /*
        Panel.SetActive(true);
        DeathTextGameObject.SetActive(true);
        LastedTextGameObject.SetActive(true);
        StabilityIncreaseGameObject.SetActive(true);
        */
        DeathScreenGameObject.SetActive(true);
    }

    public void DeathScreen(GameData gameData)
    {
        EnableDeathScreen();
        LastedText.text = $"Time Survived: {Math.Floor(gameData.timer)} seconds";
        int deltaMaxStability = gameData.IncreaseMaxStability();
        StabilityIncrease.text = $"Max Stabilty: {gameData.maxStability} (+{deltaMaxStability})";
    }

    public void BuildingTimerProgressBarUpdate(GameData gameData)
    {
        BuildingTimerProgressBar.value = (float)(gameData.building.CurrentBuilding.timeLeft / gameData.building.CurrentBuilding.buildTime);
        BuildingNameText.text = gameData.building.CurrentBuilding.name;
        BuildTimeText.text = Math.Floor(gameData.building.CurrentBuilding.timeLeft * 10) / 10 + "s/" + gameData.building.CurrentBuilding.buildTime + "s";
    }

    public void StabilityProgressBarUpdate(GameData gameData)
    {
        progressBar.value = (float)(gameData.resources[1] / gameData.maxStability);
        stabilityText.text = "Stability: " + Math.Floor(gameData.resources[1]) + " / " + gameData.maxStability;
        stabilityDropText.text = "Stability Drop: " + Math.Floor(gameData.resources[2] * 100) / 100f;
        timerText.text = "Time: " + Math.Floor(gameData.timer / 60) + ":" + Math.Floor(gameData.timer % 60);
    }

    public void SetCurrentAction(GameData gameData)// TODO: Refactor to not send whole gamedata obj, just whats needed 
    {
        CurrentActionText.text = "Current Action: " + gameData.playerOption.name;   
    }

    public void GuiChangeI(int i)
    {
            switch (i)
            {
                case 1:
                    FoodFocusButton.SetActive(true);
                    break;
                case 2:
                    UnrestFocusButton.SetActive(true);
                    break;
            }

    }
}
