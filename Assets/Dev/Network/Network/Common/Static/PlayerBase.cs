namespace _0.PersonalWork.Harry.Scripts.Common.Static
{
    public class PlayerBase
    {
        public int idVassalPlayer;
        public int idVassalTemplate;
        public int Leader;
        public int Streghth;
        public int Intelligence;
        public int Wisdom;
        public int Health;
        public int Dexterity;
        public int Charm;
        public int Eloquence;
        public int Elaborate;
        public int Analytical;
        public int Sociability;
        public int Patience;
        public int Communion;
        public int Belief;
        public int Karma;
        public int Lucky;
        public int Activity;
        public int Martime;
        public int JobClass_1;
        public int PoinClassJob_1;
        public int JobClass_2;
        public int PoinClassJob_2;
        public int JobClass_3;
        public int PoinClassJob_3;
        public int Status;
        public int Level;
        public int Loyalty;

        public PlayerBase(Fbs.LoginVassalInfo vassalInfo)
        {
            idVassalPlayer = vassalInfo.IdVassalPlayer;
            idVassalTemplate = vassalInfo.IdVassalTemplate;
            Status = vassalInfo.Status;
            Level = vassalInfo.Level;
            Loyalty = vassalInfo.Loyalty;
        }
    }
}