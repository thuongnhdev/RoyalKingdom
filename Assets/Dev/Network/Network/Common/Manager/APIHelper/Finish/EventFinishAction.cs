using CoreData.UniFlow.Commander;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Fbs;
using UnityEngine;
using static CoreData.UniFlow.Common.Character;

namespace CoreData.UniFlow.Common
{
    public class EventFinishAction : MonoSingleton<EventFinishAction>
    {
        [SerializeField] private GameEvent _onItemComplete;

        [SerializeField] private GameEvent _onFinishConstruction;

        [SerializeField] private GameEvent _onBuildingFinishUpgrade;

        [SerializeField] private GameEvent _onCanceledtDestroyProgres;

        [SerializeField] private GameEvent _onFinishBuilding;

        [SerializeField] private GameEvent _onFinishItem;

        [SerializeField] private GameEvent _onFinishUpgrade;

        [SerializeField] private GameEvent _onFinishDestroy;

        private void OnEnable()
        {
            _onItemComplete.Subcribe(OnFinishItem);
            _onFinishConstruction.Subcribe(OnFinishContruction);
            _onBuildingFinishUpgrade.Subcribe(OnFinishUpgrade);
            _onCanceledtDestroyProgres.Subcribe(OnFinishDestroy);
            
            _onFinishItem.Subcribe(ResponseFinishItem);
            _onFinishUpgrade.Subcribe(ResponseFinishUpgrade);
            _onFinishDestroy.Subcribe(ResponseFinishDestroy);
            _onFinishBuilding.Subcribe(ResponseFinishBuildding);
        }

        private void OnDisable()
        {
            _onItemComplete.Unsubcribe(OnFinishItem);
            _onFinishConstruction.Unsubcribe(OnFinishContruction);
            _onBuildingFinishUpgrade.Unsubcribe(OnFinishUpgrade);
            _onCanceledtDestroyProgres.Unsubcribe(OnFinishDestroy);

            _onFinishItem.Unsubcribe(ResponseFinishItem);
            _onFinishUpgrade.Unsubcribe(ResponseFinishUpgrade);
            _onFinishDestroy.Unsubcribe(ResponseFinishDestroy);
            _onFinishBuilding.Unsubcribe(ResponseFinishBuildding);
        }

        public void OnFinishItem(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            int buildingObjectId = (int)eventParam[0];
            var taskData =
                GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData == null) return;
            _ = APIManager.Instance.RequestFinishItem(taskData);
     
        }

        public void OnFinishContruction(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            int buildingObjectId = (int)eventParam[0];
            var taskData =
                GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData == null) return;
            _ = APIManager.Instance.RequestFinishBuilding(taskData);

        }

        public void OnFinishUpgrade(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            int buildingObjectId = (int)eventParam[0];
            var taskData =
                GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData == null) return;
            _ = APIManager.Instance.RequestFinishUpgrade(taskData);

        }

        public void OnFinishDestroy(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            int buildingObjectId = (int)eventParam[0];
            var taskData =
                GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData == null) return;
            _ = APIManager.Instance.RequestFinishDestroy(taskData);

        }

        public void ResponseFinishItem(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            ReponseFinishItem reponseFinishItem = (ReponseFinishItem)eventParam[0];
            TypeJobAction typeJob = Utils.GetTypeJobByName(reponseFinishItem.Name);
            typeJob = TypeJobAction.FINISHITEM;
            FinishItemProduction.Instance.OnFinishItemProduction(reponseFinishItem.BuildingPlayerId, typeJob);
        }

        public void ResponseFinishUpgrade(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            FinishBuildding.Instance.OnFinishBuildding(eventParam);
        }

        public void ResponseFinishDestroy(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            FinishBuildding.Instance.OnFinishBuildding(eventParam);
        }

        public void ResponseFinishBuildding(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            FinishBuildding.Instance.OnFinishBuildding(eventParam);
        }
    }
}
