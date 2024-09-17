using System;
using CoreData.UniFlow.Common;
using UnityEngine;

public enum TypeStore
{
    Stone = 1,
    Wood,
    Water,
    None
}
[Serializable]
public class DataStore
{
    public CharacterBehaviour CharacterBehaviour;
    public TypeStore TypeStore;
    public DataStore(CharacterBehaviour character, TypeStore type)
    {
        this.CharacterBehaviour = character;
        this.TypeStore = type;
    }
}

[Serializable]
public class CharacterTaskData
{
    public string Name;
    public int CharacterId;
    public int BuildingObjectId;
    public StatusAction StatusAction;
    public Character.CharacterList Type;
    public CharacterBehaviour CharacterBehaviour;
    public CharacterTaskData(string name ,int buildingObjectId,CharacterBehaviour character, StatusAction statusAction, int characterId, Character.CharacterList type)
    {
        this.Name = name;
        this.BuildingObjectId = buildingObjectId;
        this.CharacterBehaviour = character;
        this.StatusAction = statusAction;
        this.CharacterId = characterId;
        this.Type = type;
    }
}

[Serializable]
public class CallbackMoveData
{
    public int IDCharacter;
    public Vector3 Position;
    public Action<DataStore> OnComplete;

    public CallbackMoveData(int idCharacter, Vector3 position, Action<DataStore> onComplete)
    {
        this.IDCharacter = idCharacter;
        this.Position = position;
        this.OnComplete = onComplete;
    }
}