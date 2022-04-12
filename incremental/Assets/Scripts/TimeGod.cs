using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TimeGod : MonoBehaviour
{
    public class TimeTickEventArgs : EventArgs
    {
        public int timeTick;
    }
    public static event EventHandler<TimeTickEventArgs> TimeTick;
    private const float MAX = .5f;
    private int timeTick;
    private float realTime;


    // Update is called once per frame
    private void Update()
    {
        realTime += Time.deltaTime;
        if (MAX <= realTime)
        {
            realTime = realTime - MAX;
            timeTick += 1;
            if (TimeTick != null) TimeTick(this, new TimeTickEventArgs { timeTick = timeTick });
            Debug.Log("tick");
        }
    }

    private void Begin()
    {
        timeTick = 0;
    }
}
