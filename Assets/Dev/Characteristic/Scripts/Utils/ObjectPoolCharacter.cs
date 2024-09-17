using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Globalization;

namespace CoreData.UniFlow.Common
{
    [System.Serializable]
    public class Preallocation
    {
        public GameObject gameObject;
        public int count;
        public bool expandable;
        public Transform parent;
        public Character.CharacterList CType;
        public int IdVassalModel;
        public string Name;
    }

    [System.Serializable]
    public class DataPool
    {
        public int Id;
        public GameObject gameObject;
        public string timeActive;
        public DataPool(int id,GameObject obj , string timeActive)
        {
            this.Id = id;
            this.gameObject = obj;
            this.timeActive = timeActive;
        }
    }

    public class ObjectPoolCharacter : MonoSingleton<ObjectPoolCharacter>
    {
        private List<Preallocation> _preallocations = new List<Preallocation>();

        [SerializeField]
        List<DataPool> _poolVassalobjects_genghis;

        [SerializeField]
        List<DataPool> _poolVassalobjects_richard;

        [SerializeField]
        List<DataPool> _poolVassalobjects_jeanne;

        [SerializeField]
        List<DataPool> _poolVassalobjects_saladin;

        [SerializeField]
        List<DataPool> _poolVassalobjects_attila;

        [SerializeField]
        List<DataPool> _poolVassalobjects_rurik;

        [SerializeField]
        List<DataPool> _poolVassalobjects_xiang;

        [SerializeField]
        List<DataPool> _poolWorkerobjects;

        [SerializeField]
        List<DataPool> _poolAssistantobjects;

        private MasterDataStore _masterDataStore;
        public ObjectPoolCharacter Init(Preallocation preallocation, List<CharacterTaskData> characterTaskData)
        {
            _preallocations.Add(preallocation);
            if (preallocation.CType == Character.CharacterList.Vassar)
            {
                switch(preallocation.IdVassalModel)
                {
                    case 1:
                        _poolVassalobjects_genghis = new List<DataPool>();
                        break;
                    case 2:
                        _poolVassalobjects_richard = new List<DataPool>();
                        break;
                    case 3:
                        _poolVassalobjects_jeanne = new List<DataPool>();
                        break;
                    case 4:
                        _poolVassalobjects_saladin = new List<DataPool>();
                        break;
                    case 5:
                        _poolVassalobjects_attila = new List<DataPool>();
                        break;
                    case 6:
                        _poolVassalobjects_rurik = new List<DataPool>();
                        break;
                    case 7:
                        _poolVassalobjects_xiang = new List<DataPool>();
                        break;
                }
            }
            if (preallocation.CType == Character.CharacterList.Farmer && _poolWorkerobjects.Count == 0) 
                _poolWorkerobjects = new List<DataPool>();
            if(preallocation.CType == Character.CharacterList.AssistantWorker && _poolAssistantobjects.Count == 0)
                _poolAssistantobjects = new List<DataPool>();
            _masterDataStore = MasterDataStore.Instance;

            for (int i = 0; i < preallocation.count; ++i)
            {
                CreateData(i, preallocation, preallocation.gameObject, characterTaskData, false);
            }
            return this;
        }

        private CharacterTaskData CreateData(int index, Preallocation preallocation, GameObject gameObject, List<CharacterTaskData> characterTaskData, bool isActive)
        {
            int id = 0;
            string name = preallocation.Name.ToString();
            GameObject go = CreateGobject(gameObject, name , preallocation.CType);
            go.transform.parent = _preallocations.Find(t => t.CType == preallocation.CType).parent;
            var timeCurrent = System.DateTime.Now.ToString("yyyyMMddHHmmss");

            if (preallocation.CType == Character.CharacterList.Vassar)
            {
                int indexC = preallocation.IdVassalModel - 1;
                id = _masterDataStore.BaseVassalTemplates[indexC].Key;
                switch (preallocation.IdVassalModel)
                {
                    case 1:
                        _poolVassalobjects_genghis.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                    case 2:
                        _poolVassalobjects_richard.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                    case 3:
                        _poolVassalobjects_jeanne.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                    case 4:
                        _poolVassalobjects_saladin.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                    case 5:
                        _poolVassalobjects_attila.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                    case 6:
                        _poolVassalobjects_rurik.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                    case 7:
                        _poolVassalobjects_xiang.Add(new DataPool(preallocation.IdVassalModel, go, timeCurrent));
                        break;
                }
            }
            if (preallocation.CType == Character.CharacterList.Farmer)
            {
                id = _masterDataStore.WorkerDatas[index].ID + 1000;
                DataPool data = new DataPool(id, go, timeCurrent);
                _poolWorkerobjects.Add(data);
            }
            if (preallocation.CType == Character.CharacterList.AssistantWorker)
            {
                int count = _poolAssistantobjects.Count;
                id = _masterDataStore.WorkerDatas[count + 1].ID + 9999;
                DataPool data = new DataPool(id, go, timeCurrent);
                _poolAssistantobjects.Add(data);
            }

            CharacterBehaviour character = go.GetComponent<CharacterBehaviour>();
            character.gameObject.SetActive(isActive);
            CharacterTaskData taskData = new CharacterTaskData(name, -1, character, StatusAction.None, id, preallocation.CType);
            character.InitData(taskData);
            characterTaskData.Add(taskData);
            return taskData;
        }
        public CharacterTaskData SpawnVassal(int id, List<CharacterTaskData> characterTaskData, Character.CharacterList cType)
        {
            List<DataPool> listData = _poolWorkerobjects;
            if (cType == Character.CharacterList.Vassar)
            {
                switch (id)
                {
                    case 1:
                        listData = _poolVassalobjects_genghis;
                        break;
                    case 2:
                        listData = _poolVassalobjects_richard;
                        break;
                    case 3:
                        listData = _poolVassalobjects_jeanne;
                        break;
                    case 4:
                        listData = _poolVassalobjects_saladin;
                        break;
                    case 5:
                        listData = _poolVassalobjects_attila;
                        break;
                    case 6:
                        listData = _poolVassalobjects_rurik;
                        break;
                    case 7:
                        listData = _poolVassalobjects_xiang;
                        break;
                }

            }
            for (int i = 0; i < listData.Count; ++i)
            {
                var timeCurrent = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                listData[i].timeActive = timeCurrent;
                CharacterTaskData taskData = characterTaskData.Find(t => t.CharacterId == id);
                if (taskData != null)
                {
                    listData[i].gameObject.SetActive(true);
                    return taskData;
                }
            }
            return null;
        }

        public CharacterTaskData Spawn(List<CharacterTaskData> characterTaskData , Character.CharacterList cType)
        {
            int id = 0;
            List<DataPool> listData = _poolWorkerobjects;
            if(cType == Character.CharacterList.Farmer)
            {
                id = 1000;
                listData = _poolWorkerobjects;
            }
            else if (cType == Character.CharacterList.AssistantWorker)
            {
                id = 10000;
                listData = _poolAssistantobjects;
            }
            for (int i = 0; i < listData.Count; ++i)
            {
                if (!listData[i].gameObject.activeSelf)
                {
                    var timeCurrent = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                    listData[i].timeActive = timeCurrent;
                    CharacterTaskData taskData = characterTaskData.Find(t => t.CharacterId == (id + i));
                    if (taskData != null)
                    {
                        listData[i].gameObject.SetActive(true);
                        return taskData;
                    }
                }
            }
            var preallocation = _preallocations.Find(t => t.CType == cType);
            for (int i = 0; i < preallocation.count; ++i)
            {
                if (preallocation.expandable)
                {
                    return CreateData(i, preallocation, preallocation.gameObject, characterTaskData, true);
                }
            }
            return null;
        }

        GameObject CreateGobject(GameObject item,string name, Character.CharacterList cType)
        {
            GameObject gobject = Instantiate(item, transform);
            gobject.name = name;
            if (cType == Character.CharacterList.Vassar)
                gobject.SetActive(true);
            else
                gobject.SetActive(false);
            return gobject;
        }

        public void DisableObject(CharacterTaskData characterTaskData)
        {
            characterTaskData.CharacterBehaviour.gameObject.SetActive(false);
            //for (int i = 0; i < _pooledGobjects.Count; ++i)
            //{
            //    if (_pooledGobjects[i].gameObject.activeSelf)
            //    {
            //        CharacterBehaviour character = _pooledGobjects[i].gameObject.GetComponent<CharacterBehaviour>();
            //        CharacterTaskData taskData = character.CharacterTaskData;
            //        if (taskData.CharacterId == characterTaskData.CharacterId)
            //        {
            //            _pooledGobjects[i].gameObject.SetActive(false);
            //            var timeCurrent = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            //            _pooledGobjects[i].timeActive = timeCurrent;
            //        }
            //    }
            //}
        }

        private int _countUpdate = 0;
        private void FixedUpdate()
        {
            _countUpdate++;
            if (_countUpdate == 8000)
            {
                _countUpdate = 0;
                for (int i = 0; i < _poolWorkerobjects.Count; ++i)
                {
                    if (!_poolWorkerobjects[i].gameObject.activeSelf)
                    {
                        DateTime timeCurrent = DateTime.ParseExact(_poolWorkerobjects[i].timeActive, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                        System.TimeSpan diff1 = DateTime.Now.Subtract(timeCurrent);
                        if (diff1.TotalMinutes > 10)
                        {
                            Destroy(_poolWorkerobjects[i].gameObject);
                            _poolWorkerobjects.RemoveAt(i);
                        }
                    }
                }
            }
        }

    }
}