namespace _0.PersonalWork.Harry.Scripts.Common.Static
{
    public class CitizenBase
    {
        public int idPopulationCitizen;
        public int idPopulationPlayer;
        public int CitizenJobId;
        public int Age;
        public int Gender;

        public CitizenBase(Fbs.LoginInfoPopulationCitzen citizenBase)
        {
            idPopulationCitizen = citizenBase.IdPopulationCitizen;
            idPopulationPlayer = citizenBase.IdPopulationPlayer;
            CitizenJobId = citizenBase.CitizenJobId;
            Age = citizenBase.Age;
            Gender = citizenBase.Gender;
        }
    }
}