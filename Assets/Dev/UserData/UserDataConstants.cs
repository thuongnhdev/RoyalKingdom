

public enum BuildingStatus
{
    None = -1,
    WaitForConstructingResource = 0,
    OnConstruction = 1,
    Idle = 2,
    WaitingForUpgradeResource = 3,
    Upgrading = 4,
    WaitingForProductResource = 5,
    Producing = 6,
    Training = 7,
    OnDestruction = 8,
    Damaged = 9,
    WaitForRepairResource = 10,
    Repairing = 11,
    Destructed = 12
}
