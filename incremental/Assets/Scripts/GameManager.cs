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
        Timer();
        gui.TickUpdate(data);
    }

    private void Timer()
    {
        data.timer += 0.5f;
    }
    
    private void StabilityCalc()
    {
        GameData.stabilityDrop += GameData.stabilityDrop * 0.002f;
        GameData.stability -= GameData.stabilityDrop;
    }

    private void PopulationCalc()
    {
        data.population += data.population * 0.002;
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
    public static float stability;
    public static float stabilityDrop;
    public static float maxStability;
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
        stabilityDrop = 0.10f;
        food = 10;
    }

    public void HardReset()
    {
        population = 100;
        stability = 50f;
        stabilityDrop = 0.10f;
        maxStability = 50f;
        food = 10;
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
    public Slider progressBar;

    /* Commenting out for now as unlikely to have this generic class actually declaring stuff
    public PlayerOption(String n, String desc, double minE, double currentE, Slider pB)
    {
        name = n;
        description = desc;
        minEmployment = minE;
        progressBar = pB;
        currentEmployment = 0;
    }*/
}

public class Building : PlayerOption 
{
    int buildTime;
    double timeLeft;
    double[] bonus;

    //Sets superclass of PlayerOption stuff then specific params for building subclass.
    public Building(String n, String desc, double minE, Slider pB, int buildT, double[] b)
    {
        name = n;
        description = desc;
        minEmployment = minE;
        progressBar = pB;
        buildTime = buildT;
        bonus = b;
        timeLeft = buildTime; //This is because it will always start completely unbuilt
    }
}

public class CombatOption: PlayerOption
{
    double health;
    bool isActiveVar;   

    public CombatOption(String n, String desc, double minE, Slider pB, double h)
    {
        name = n;
        description = desc;
        minEmployment = minE;
        progressBar = pB;
        health = h;
        isActiveVar = false;
    }

    public bool isActive()
    {
        isActiveVar = health >= 0; //Condition to still remain active, will also activate this if not active!
        return isActiveVar;
    }
}

public class Job : PlayerOption
{
    double[] resourceDelta;

    public Job(String n, String desc, double minE, Slider pB, double[] resourceD)
    {
        name = n;
        description = desc;
        minEmployment = minE;
        progressBar = pB;
        resourceDelta = resourceD;
    }
}



