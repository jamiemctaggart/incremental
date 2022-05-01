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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TickUpdate(GameData gameData)
    {
        foodText.text = "Food: " + gameData.resources[0];
        populationText.text = "Population: " + Math.Floor(gameData.population);
        //Progress bar stuff, value updates and the text to show the 3 values
        StabilityProgressBarUpdate(gameData);
        BuildingTimerProgressBarUpdate();
        traditionText.text = "Tradition: " + gameData.tradition;

    }

    public void BuildingTimerProgressBarUpdate()
    {
        //BuildingNameText.text = //TODO finish after sorting 
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
}
