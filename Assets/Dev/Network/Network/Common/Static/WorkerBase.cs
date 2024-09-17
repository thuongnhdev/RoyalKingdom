namespace _0.PersonalWork.Harry.Scripts.Common.Static
{
    public class WorkerBase
    {
        public int idPopulationWorker;
        public int idPopulationPlayer;
        public int CitizenJobId;
        public int Age;
        public int MoodId;

        public WorkerBase(Fbs.LoginInfoPopulationWorker workerBase)
        {
            idPopulationWorker = workerBase.IdPopulationWorker;
            idPopulationPlayer = workerBase.IdPopulationPlayer;
            CitizenJobId = workerBase.CitizenJobId;
            Age = workerBase.Age;
            MoodId = workerBase.MoodId;
        }
    }
}