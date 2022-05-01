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
        } else
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
        if (gameData.playerOption.resourceDelta[3] > 0)
        {
            gameData.CurrentNextBuilding.BuildTick(gameData.playerOption.resourceDelta[3]);
            gui.BuildingTimerProgressBarUpdate(gameData);
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
                FileStream file =
                    File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
                //gameData = null;
                gameData = (GameData)binaryFormatter.Deserialize(file);
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

[Serializable]
public class GameData
{
    public double population;
    public double maxStability;
    public double food;
    public float timer;
    public int tradition;
    public bool hasUnrest;
    public bool internalCalm;
    public string[] resourcesNames;
    public double[] resources;
    public Building CurrentNextBuilding;
    public PlayerOption playerOption;

    public GameData()
    {
        HardReset();
    }

    public void Reset()
    {
        CurrentNextBuilding = new Building("Courthouse", "Allows Unrest Management", 100, 120, new double[] { 1, 0.1, 0, 0.2});
        population = 100;
        internalCalm = false;// When true, stability will improve
        resourcesNames = new string[] { "Food", "Stability", "stabilityDrop", "Build Time" };
        resources = new double[] { 100, 50, 0.1, 60 };
        playerOption = new PlayerOption("Idle", "Does nothing", 0, 0, new double[] { 0, 0, 0, 0 });
    }

    // All soft Reset stuff PLUS tradition and prestige things
    public void HardReset()
    {
        Reset();
        maxStability = 50;
        tradition = 0;
    }
} 

public class UnrestEventController
{
    public bool status;//If true in unrest event, otherwise just building
    public double tickRate; //Speed at which unrest event will be gained
    //TODO continue from here finish the unrest controller and work back up to the game controller
    public UnrestEvent[] allUnrestEventsChronological;
    public int unrestEventI = 0;
}

public class UnrestEvent
{
    public double damage;
    public String name;
    public PlayerOption[] unrestOptions;//Array of player options *during* unrest event
    public PlayerOption[] rewardOptions;//Array of player option rewards after unrest event
    public CombatOption combatOption;

    bool UnrestUpdate()// Event controller will first check this to see if it is won, if so will then run UnrestDelta
    {
        return combatOption.isActive();//If active, then keep the same, otherwise remove unrestOptions and add rewardOptions
    }
}

[Serializable]
public class PlayerOption
{
    public String name;
    public String description;
    public double minEmployment;
    public double currentEmployment;
    public double[] resourceDelta;

    public PlayerOption()
    {

    }

    public PlayerOption(String n, String desc, double minE, double currentE, double[] resourceD)
    {
        name = n;
        description = desc;
        minEmployment = minE;
        currentEmployment = currentE;
        resourceDelta = resourceD;
    }
}

//Refactor class to fit new playeroption stuff
[Serializable]
public class Building : PlayerOption 
{
    public int buildTime;
    public double timeLeft;
    double[] bonus;

    //Sets superclass of PlayerOption stuff then specific params for building subclass.
    public Building(String n, String desc, double minE, int buildT, double[] b): base(n, desc, minE, 0, new double[] { 0, 0, 0, 0 })
    {
        buildTime = buildT;
        bonus = b;
        timeLeft = buildTime; //This is because it will always start completely unbuilt
    }

    public void BuildTick(double tickDecrement)
    {
        timeLeft -= tickDecrement;// TODO Perhaps change to a bool return and check if building was finished?
    }
}

// Refactor
public class CombatOption: PlayerOption
{
    double health;
    bool isActiveVar;   

    public CombatOption(String n, String desc, double minE, double h): base(n, desc, minE, 0, new double[] {0,0,0,0})
    {
        health = h;
        isActiveVar = false;
    }

    public bool isActive()
    {
        isActiveVar = health >= 0; //Condition to still remain active, will also activate this if not active!
        return isActiveVar;
    }
}


