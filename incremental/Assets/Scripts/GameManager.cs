using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public GameData data;

    public TextMeshProUGUI foodText;
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI stabilityText;
    public TextMeshProUGUI stabilityDropText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI traditionText;
    
    // Start is called before the first frame update
    void Start()
    {
        //LoadGame();
        NewGame();
    }

    void NewGame()
    {
        data = new GameData();
    }


    // Update is called once per frame
    void Update()
    {
        foodText.text = "Food: " + data.food;
        populationText.text = "Population: " + Math.Floor(data.population);
        stabilityText.text = "Stability: " + Math.Floor(data.stability * 100) / 100;
        stabilityDropText.text = "Stability Drop: " + Math.Floor(data.stabilityDrop * 100) / 100;
        timerText.text = "Time: " + Math.Floor(data.timer / 60) + ":" + Math.Floor(data.timer % 60);
        traditionText.text = "Tradition: " + data.tradition;
        PopulationCalc();
        StabilityCalc();
        Timer();
    }

    private void Timer()
    {
        data.timer += Time.deltaTime;
    }
    
    private void StabilityCalc()
    {
        data.stabilityDrop += data.stabilityDrop * 0.002 * Time.deltaTime;
        data.stability -= data.stabilityDrop * Time.deltaTime;
    }

    private void PopulationCalc()
    {
        data.population += data.population * 0.002 * Time.deltaTime;
    }
    
    void SaveGame()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat"); 
        binaryFormatter.Serialize(file, data);
        file.Close();
        Debug.Log("Game saved.");
    }

    void LoadGame()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file =
                    File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
                data = (GameData)bf.Deserialize(file);
                file.Close();// Closes file
                Debug.Log("Game loaded.");
            }
            else
            {
                Debug.LogError("There is no save data, creating an empty save");
                NewGame(); //Starts game data from reset() values
            }
        } catch
        {
            Debug.LogError("Save data is from old version of game");
            NewGame(); //Starts game data from reset() values
        }
    }
}

[Serializable]
public class GameData
{
    public double population;
    public double stability;
    public double stabilityDrop;
    public double food;
    public float timer;
    public int tradition;
    public bool hasUnrest;

    public GameData()
    {
        HardReset();
    }

    public void Reset()
    {
        population = 100;
        stability = 50;
        stabilityDrop = 0.10;
        food = 10;
    }

    public void HardReset()
    {
        population = 100;
        stability = 50;
        stabilityDrop = 0.10;
        food = 10;
        tradition = 0;
    }

}
