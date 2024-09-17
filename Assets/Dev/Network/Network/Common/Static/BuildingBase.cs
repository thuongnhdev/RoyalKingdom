namespace _0.PersonalWork.Harry.Scripts.Common.Static
{
    public class BuildingBase
    {
        private int BuildingPlayerId;
        private int BuildingTemplateId;
        private int Location;
        private int Rotation;
        private int Status;
        private int Level;

        public BuildingBase(Fbs.LoginBuildingInfo buildingInfo)
        {
            BuildingPlayerId = buildingInfo.BuildingPlayerId;
            BuildingTemplateId = buildingInfo.BuildingTemplateId;
            Location = buildingInfo.Location;
            Rotation = buildingInfo.Rotation;
            Status = buildingInfo.Status;
            Level = buildingInfo.Level;
        }
    }
}