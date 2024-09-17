using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class TriggerTimer : MonoBehaviour
    {

        public static event System.Action<BuildingTimeData> OnUpdateListHarvest = delegate { };
        private readonly Queue<UnityTimer> _timers = new Queue<UnityTimer>();
        public void AddTaskTimer(float duration,TaskData taskData = null, BuildingTimeData buildItem = null, Action<int> onComplete = null)
        {
            if (duration <= 0) return;
            if(buildItem == null)
            {
                buildItem = new BuildingTimeData(taskData.BuildingObjectId, -1, "", "", "", "");
            }
            _timers.Enqueue(new UnityTimer(duration, taskData, buildItem, (item) =>
             {
                 onComplete?.Invoke(item.BuildingObjectId);
                 if (buildItem.TimeDuration != "") OnUpdateListHarvest?.Invoke(item);
             }
                   ));
        }

        public void EventUpdateListHarvest(BuildingTimeData buildItem) => OnUpdateListHarvest?.Invoke(buildItem);

        public void Update()
        {
            if (_timers.Count > 0)
            {
                if (_timers.Peek().Finished)
                {
                    var timer = _timers.Dequeue();
                    timer.Dispose();
                    // Continue
                    if (_timers.Count > 0)
                    {
                        _timers.Peek().Start();
                    }
                }
                else if (!_timers.Peek().Started)
                {
                    _timers.Peek().Start();
                }
            }
        }
    }
}