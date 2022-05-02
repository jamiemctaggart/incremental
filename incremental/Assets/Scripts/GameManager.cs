using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameData gameData;
    public GUI gui;
    // Start is called before the first frame update
    void Start()
    {
        NewGame();
        Debug.Log("STARTING UP");
        TimeGod.TimeTick += delegate (object sender, TimeGod.TimeTickEventArgs e)
        {
            TickUpdate();
        };
        //LoadGame();


    }

    void NewGame()
    {
        gameData = new GameData();
    }


    // Update is called once per frame
    void Update()
    {
    }


    void TickUpdate()
    {
        PopulationCalc();
        StabilityCalc();
        ResourcesCalc();
        Timer();
        gui.TickUpdate(gameData);
    }

    private void Timer()
    {
        gameData.timer += 0.2f;
    }

    private void StabilityCalc()
    {
        //resources[1] = current stability, resources[2] = stabilityDrop
        // If not internal calm, then increase stability decay
        if (!gameData.internalCalm)
            gameData.resources[2] += gameData.resources[2] * 0.002;
        // If internal calm is currently true AND stability is not being improved, remove internal calm
        else if (gameData.playerOption.resourceDelta[1] == 0)
            gameData.internalCalm = false;
        gameData.resources[1] -= gameData.resources[2] * 0.2;
    }

    private void PopulationCalc()
    {
        double consumption = gameData.population * 0.001;
        if (consumption <= gameData.resources[0])
        {
            gameData.resources[0] -= consumption;
            gameData.population += gameData.population * 0.002;
        }
        else
        {
            // Enter starvation mode where population degrades and stability decays faster and increases decay growth
            gameData.population -= gameData.population * 0.005;
            StabilityCalc();
        }
    }

    private void ResourcesCalc()
    {
        for (int i = 0; i < gameData.resources.Length; i++)
            gameData.resources[i] += gameData.playerOption.resourceDelta[i];
        // If stability is above max, cap at the max.
        if (gameData.resources[1] > gameData.maxStability)
            gameData.resources[1] = gameData.maxStability;
        // If currently building, run the build calc function, seperated
        // for easier reading of code.
        if (gameData.playerOption.resourceDelta[3] > 0)
            BuildCalc();
    }

    private void BuildCalc()
    {
        // If returns true, it means its moved to a new building!, so change gui
        if (gameData.building.CurrentBuilding.BuildTick(gameData.playerOption.resourceDelta[3]))
        {
            gameData.building.NextBuilding();
            gui.BuildingTimerProgressBarUpdate(gameData);
            //GUI switch
            gui.GuiChangeI(gameData.building.BuildingI);
        }
    }

    public void SaveGame()
    {
        Debug.Log("Started Save...");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        binaryFormatter.Serialize(file, gameData);
        file.Close();
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
                //gameData = null;
                gameData = (GameData)binaryFormatter.Deserialize(file);
                file.Close();// Closes file
                //If BuildingI > 0 then iterate through every gui change that would occur
                gui.Start();
                for (int i = 1; i <= gameData.building.BuildingI; i++)
                    gui.GuiChangeI(i);

                Debug.Log("BuildingI:" + gameData.building.BuildingI);
            }
            else
            {
                Debug.LogError("There is no save data, creating an empty save");
                NewGame(); //Starts game data from reset() values
            }
        }
        catch
        {
            Debug.LogError("Save data is from old version of game");
            NewGame(); //Starts game data from reset() values
        }
    }

    public void SetFoodFocus()
    {
        double[] resourceDelta = new double[] { 0.5, 0, 0, 0 };
        gameData.playerOption = new PlayerOption("Food Focus", "Focuses the populace on farming and other food production methods", 100, gameData.population, resourceDelta);
        gui.SetCurrentAction(gameData);
    }

    public void SetUnrestFocus()
    {
        gameData.internalCalm = true;
        double[] resourceDelta = new double[] { 0, 1, 0, 0 };
        gameData.playerOption = new PlayerOption("Unrest Focus", "Focuses the populace on keeping the peace and plataus the stability drop", 100, gameData.population, resourceDelta);
        gui.SetCurrentAction(gameData);
    }

    public void SetBuildFocus()
    {
        double[] resourceDelta = new double[] { 0, 0, 0, 0.2 };
        gameData.playerOption = new PlayerOption("Build Focus", "Focuses on improving the capital", 100, gameData.population, resourceDelta);
        gui.SetCurrentAction(gameData);
    }
}



