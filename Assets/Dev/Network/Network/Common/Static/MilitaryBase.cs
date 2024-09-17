namespace _0.PersonalWork.Harry.Scripts.Common.Static
{
    public class MilitaryBase
    {
        public int idPopulationMilitary;
        public int idPopulationPlayer;
        public int CitizenJobId;
        public int Age;

        public MilitaryBase(Fbs.LoginInfoPopulationMilitary militaryBase)
        {
            idPopulationMilitary = militaryBase.IdPopulationMilitary;
            idPopulationPlayer = militaryBase.IdPopulationPlayer;
            CitizenJobId = militaryBase.CitizenJobId;
            Age = militaryBase.Age;
        }
    }
}