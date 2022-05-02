using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
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

    // Start is called before the first frame update
    public void Start()
    {
        FoodFocusButton.SetActive(false);
        UnrestFocusButton.SetActive(false);
    }

    public void TickUpdate(GameData gameData)
    {
        foodText.text = "Food: " + gameData.resources[0];
        populationText.text = "Population: " + Math.Floor(gameData.population);
        //Progress bar stuff, value updates and the text to show the 3 values
        StabilityProgressBarUpdate(gameData);
        BuildingTimerProgressBarUpdate(gameData);
        traditionText.text = "Tradition: " + gameData.tradition;

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
