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

[Serializable]
public class GameData
{
    public double population;
    public double maxStability;
    public double food;
    public double consumption;
    public float timer;
    public int tradition;
    public bool hasUnrest;
    public bool internalCalm;
    public bool isDead;
    public string[] resourcesNames;
    public double[] resources;
    public double[] resourceDelta;
    public Buildings building;
    public PlayerOption playerOption;

    public GameData()
    {
        HardReset();
    }

    // All soft Reset stuff PLUS tradition and prestige things
    public void HardReset()
    {
        maxStability = 1;
        tradition = 0;
        Reset();
    }

    public void Reset()
    {
        isDead = false;
        building = new Buildings(tradition);
        population = 100;
        timer = 0f;
        internalCalm = false;// When true, stability will improve
        resourcesNames = new string[] { "Food", "Stability", "stabilityDrop", "Build Time" };
        resourceDelta = new double[] { 0, 0, 0, 0 };
        Debug.Log(maxStability);
        resources = new double[] { 100, maxStability, 0.1, 60 };//TODO revert index 1 to 50
        playerOption = new PlayerOption("Idle", "Does nothing", 0, 0, new double[] { 0, 0, 0, 0 });
    }

    public void IncreaseResourceDelta(double[] resourceD)
    {
        for (int i = 0; i < resourceDelta.Length; i++)
            resourceDelta[i] += resourceD[i];
    }

    public int IncreaseMaxStability()
    {
        int deltaMaxStability = (int)Math.Floor(Math.Log(timer * timer));
        maxStability += deltaMaxStability;
        return deltaMaxStability;
    }

    public int IncreaseTradition()
    {
        int deltaTradition = (int)Math.Floor(Math.Log(timer));
        tradition += deltaTradition;
        return deltaTradition;
    }

    
    public void PopulationCalc()
    {
        consumption = population * 0.001;
        if (consumption <= resources[0])
        {
            resources[0] -= consumption;
            population += population * 0.002;
        }
        else
        {
            // Enter starvation mode where population degrades
            population -= population * 0.005;
        }
    }

}

public class UnrestEventController
{
    public bool status;//If true in unrest event, otherwise just building
    public double tickRate; //Speed at which unrest event will be gained
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
    //public Building[] AllBuildings;
    public double[] bonus;

    //Sets superclass of PlayerOption stuff then specific params for building subclass.
    public Building(String n, String desc, double minE, int buildT, double[] b) : base(n, desc, minE, 0, new double[] { 0, 0, 0, 0 })
    {
        buildTime = buildT;
        bonus = b;
        timeLeft = buildTime; //This is because it will always start completely unbuilt
    }

    //Returns true if completed building, false if not
    public bool BuildTick(double tickDecrement)
    {
        timeLeft -= tickDecrement;
        return 0 >= timeLeft;
    }
}

// Class to control current building and list of all buildings
[Serializable]
public class Buildings
{
    public static IList<Building> AllBuildings;
    public Building CurrentBuilding;
    //For keeping track of gui unlockable actions
    public int BuildingI;

    public Buildings(int tradition)
    {
        AllBuildings = new List<Building>()
        {
            new Building("Basic Farm(+1 food)", "More efficient farming setup", 100, buildTime(60, tradition), new double[] { 1, 0, 0, 0}),// TODO give useful resource deltas here later when the system is more worked out
            new Building("Courthouse", "Allows Unrest Management", 100, buildTime(50, tradition), new double[] { 0, 0.1, 0, 0.2}),
            new Building("Expand Farm(+5 food)", "More efficient farming setup", 100, buildTime(70,tradition), new double[] { 5, 0, 0, 0}),// TODO give useful resource deltas here later when the system is more worked out
            new Building("Builder", "Auto Builds", 100, buildTime(180, tradition), new double[] { 0, 0, 0, 1}),// TODO give useful resource deltas here later when the system is more worked out
            new Building("Barracks", "Allows military build up", 100, buildTime(150, tradition), new double[] { 0, 0, 0, 0})// TODO give useful resource deltas here later when the system is more worked out
        };
        CurrentBuilding = AllBuildings[0];
        BuildingI = 0;
    }

    private int buildTime(int originalSeconds, int tradition)
    {
        Debug.Log(Math.Floor(originalSeconds / (1.0 + (tradition / 100.0))));
        return (int)Math.Floor(originalSeconds / (1.0 + (tradition / 100.0)));
    }

    public void NextBuilding()
    {
        //Remove head of AllBuildings array here
        if (AllBuildings.Count > 1)
        {
            BuildingI++;
            Debug.Log("Removing " + CurrentBuilding.name);
            AllBuildings.RemoveAt(0);//TODO add null check here ofc
            CurrentBuilding = AllBuildings[0];
            Debug.Log("No. of buildings left: " + AllBuildings.Count);
        }
        else
        {
            AllBuildings[0] = new Building("You reached the end of all buildings(for now)", "Nothing", 100, 100000, new double[] { 0, 0, 0, 0 });
        }
    }
}

// Refactor
public class CombatOption : PlayerOption
{
    double health;
    bool isActiveVar;

    public CombatOption(String n, String desc, double minE, double h) : base(n, desc, minE, 0, new double[] { 0, 0, 0, 0 })
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
