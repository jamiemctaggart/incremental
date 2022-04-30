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
    public TextMeshProUGUI stabilityText;
    public TextMeshProUGUI stabilityDropText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI traditionText;
    public TextMeshProUGUI CurrentActionText;
    public Slider progressBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TickUpdate()
    {
        foodText.text = "Food: " + GameData.resources[0];
        populationText.text = "Population: " + Math.Floor(GameData.population);
        //Progress bar stuff, value updates and the text to show the 3 values
        progressBar.value = (float)(GameData.resources[1] / GameData.maxStability);
        stabilityText.text = "Stability: " + Math.Floor(GameData.resources[1]) + " / " + GameData.maxStability;
        stabilityDropText.text = "Stability Drop: " + Math.Floor(GameData.resources[2] * 100) / 100f;
        timerText.text = "Time: " + Math.Floor(GameData.timer / 60) + ":" + Math.Floor(GameData.timer % 60);
        traditionText.text = "Tradition: " + GameData.tradition;
    }

    public void SetCurrentAction()
    {
        CurrentActionText.text = "Current Action: " + GameData.playerOption.name;   
    }
}
