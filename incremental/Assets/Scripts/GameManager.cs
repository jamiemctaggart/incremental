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
    public GameData data;
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
        data = new GameData();
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
        gui.TickUpdate();
    }

    private void Timer()
    {
        GameData.timer += 0.2f;
    }
    
    private void StabilityCalc()
    {
        //resources[1] = current stability, resources[2] = stabilityDrop
        // If not internal calm, then increase stability decay
        if (!GameData.internalCalm)
            GameData.resources[2] += GameData.resources[2] * 0.002;
        // If internal calm is currently true AND stability is not being improved, remove internal calm
        else if (GameData.playerOption.resourceDelta[1] == 0)
            GameData.internalCalm = false;
        GameData.resources[1] -= GameData.resources[2] * 0.2;
    }

    private void PopulationCalc()
    {
        Double consumption = GameData.population * 0.001;
        if (consumption <= GameData.resources[0])
        {
            GameData.resources[0] -= consumption;
            GameData.population += GameData.population * 0.002;
        } else
        {
            // Enter starvation mode where population degrades and stability decays faster and increases decay growth
            GameData.population -= GameData.population * 0.005;
            StabilityCalc();
        }
    }

    private void ResourcesCalc()
    {
        for (int i = 0; i < GameData.resources.Length; i++)
            GameData.resources[i] += GameData.playerOption.resourceDelta[i];
        // If stability is above max, cap at the max.
        if (GameData.resources[1] > GameData.maxStability)
            GameData.resources[1] = GameData.maxStability;
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

    public void SetFoodFocus()
    {
        double[] resourceDelta = new double[] { 0.5, 0, 0, 0 };
        GameData.playerOption = new PlayerOption("Food Focus", "Focuses the populace on farming and other food production methods", 100, GameData.population, resourceDelta);
        gui.SetCurrentAction();
    }

    public void SetUnrestFocus()
    {
        GameData.internalCalm = true;
        double[] resourceDelta = new double[] { 0, 1, 0, 0 };
        GameData.playerOption = new PlayerOption("Unrest Focus", "Focuses the populace on keeping the peace and plataus the stability drop", 100, GameData.population, resourceDelta);
        gui.SetCurrentAction();
    }
}

[Serializable]
public class GameData
{
    public static double population;
    public static double maxStability;
    public static double food;
    public static float timer;
    public static int tradition;
    public static bool hasUnrest;
    public static bool internalCalm;
    public static string[] resourcesNames;
    public static double[] resources;
    public static PlayerOption playerOption;

    public GameData()
    {
        HardReset();
    }

    public void Reset()
    {
        population = 100;
        internalCalm = false;// When true, stability will improve
        resourcesNames = new string[] { "Food", "Stability", "stabilityDrop", "Build" };
        resources = new double[] { 100, 50, 0.1, 0 };
        playerOption = new PlayerOption("Idle", "Does nothing", 0, 0, new double[] { 0, 0, 0, 0 });
    }

    // Reset PLUS tradition
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
public class Building : PlayerOption 
{
    int buildTime;
    double timeLeft;
    double[] bonus;

    //Sets superclass of PlayerOption stuff then specific params for building subclass.
    public Building(String n, String desc, double minE, int buildT, double[] b): base(n, desc, minE, 0, new double[] { 0, 0, 0, 0 })
    {
        buildTime = buildT;
        bonus = b;
        timeLeft = buildTime; //This is because it will always start completely unbuilt
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


