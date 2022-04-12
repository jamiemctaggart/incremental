using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TimeGod : MonoBehaviour
{
    public class TimeTickEventArgs : EventArgs
    {
        public int time;
    }
    private const float MAX = .5f;
    private int time;
    private float realTime;
    public static event EventHandler<TimeTickEventArgs> TimeTick;

    // Update is called once per frame
    private void Update()
    {
        realTime += Time.deltaTime;
        if (MAX < realTime)
        {
            realTime = 0;
            time += 1;
            if (TimeTick != null) TimeTick(this, new TimeTickEventArgs { time = time });
            Debug.LogError("tick");
        }
    }

    private void Begin()
    {
        time = 0;
    }


}
