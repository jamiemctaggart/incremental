using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public double population;
    public double stability;
    public double stabilityDrop;
    public double food;

    public Data()
    {
        Reset();
    }

    public void Reset()
    {
        population = 100;
        stability = 50;
        stabilityDrop = 0;
        food = 10;
    }
}

public class NewBehaviourScript : MonoBehaviour
{
    public Data data;
    // Start is called before the first frame update
    void Start()
    {
        Data data = Data();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
