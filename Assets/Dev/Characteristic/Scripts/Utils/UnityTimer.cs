using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class UnityTimer : IDisposable
    {
        public bool Finished = false;
        public bool Started = false;
        private readonly Timer _timer;
        public UnityTimer(float time, TaskData taskData, BuildingTimeData buildingTimeData, Action<BuildingTimeData> actionToBeExecuted)
        {
            _timer = new Timer
            {
                Interval = time * 1000, // in milliseconds
                AutoReset = false,
                Enabled = true
        };
            _timer.Elapsed += (o, args) =>
            {
                actionToBeExecuted(buildingTimeData);
                Finished = true;
            };
        }
        public void Start()
        {
            Started = true;
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}