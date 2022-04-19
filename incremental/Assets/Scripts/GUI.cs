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
    public Slider progressBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TickUpdate(GameData data)
    {
        foodText.text = "Food: " + data.food;
        populationText.text = "Population: " + Math.Floor(data.population);
        stabilityText.text = "Stability: " + Math.Floor(GameData.stability * 100) / 100;
        stabilityDropText.text = "Stability Drop: " + Math.Floor(GameData.stabilityDrop * 100) / 100;
        timerText.text = "Time: " + Math.Floor(data.timer / 60) + ":" + Math.Floor(data.timer % 60);
        traditionText.text = "Tradition: " + data.tradition;
        progressBar.value = GameData.stability / GameData.maxStability;
    }
}
