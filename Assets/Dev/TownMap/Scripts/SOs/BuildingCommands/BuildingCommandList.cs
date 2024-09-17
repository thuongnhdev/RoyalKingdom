using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingCommands", menuName = "Uniflow/Building/Commmand/CommandList")]
public class BuildingCommandList : ScriptableObject
{
    [SerializeField]
    private List<BuildingCommand> _commands;
    private Dictionary<int, BuildingCommand> _commandDict = new();
    private Dictionary<int, BuildingCommand> CommandDict
    {
        get
        {
            if (_commands.Count != _commandDict.Count)
            {
                _commandDict.Clear();
                for (int i = 0; i < _commands.Count; i++)
                {
                    _commandDict[(int)_commands[i].commandKey] = _commands[i];
                }
            }

            return _commandDict;
        }
    }

    public Sprite GetCommandIcon(BuildingCommandKey commandKey)
    {
        var command = GetBuildingCommand(commandKey);
        if (command == null)
        {
            return null;
        }

        return command.commandIcon;
    }

    public void ExecuteBuildingCommand(BuildingCommandKey commandKey)
    {
        var command = GetBuildingCommand(commandKey);
        if (command == null)
        {
            return;
        }

        command.commandEvent.Raise();
    }

    private BuildingCommand GetBuildingCommand(BuildingCommandKey commandKey)
    {
        CommandDict.TryGetValue((int)commandKey, out BuildingCommand command);
        if (command == null)
        {
            Debug.Log($"Building with Command Key [{commandKey}] has no command");
        }

        return command;
    }
}

[System.Serializable]
public class BuildingCommand
{
    public BuildingCommandKey commandKey;
    public Sprite commandIcon;
    public GameEvent commandEvent;
}

public enum BuildingCommandKey
{
    None = 0,
    Produce = 1,
    Building = 2,
    Research = 3,
    Religion = 4,
    Purchase = 5,
    Item = 6,
    Farm = 7,
    Heal = 8,
    Repair = 9,
    Train = 10,
    Population = 11,
    ManageTroop = 12,
    Population_Military = 13
}
