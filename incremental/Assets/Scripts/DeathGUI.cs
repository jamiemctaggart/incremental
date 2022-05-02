using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathGUI : MonoBehaviour
{
    public TextMeshProUGUI foodText;

    public void Load(GameData gameData) 
    {
        foodText.text = $"You survived a total of {Math.Floor(gameData.timer)} seconds giving you +{gameData.IncreaseMaxStability()} bringing your max stability to {gameData.maxStability}";
    }
}
