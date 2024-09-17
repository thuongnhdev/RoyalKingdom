using CoreData.UniFlow.Commander;
using CoreData.UniFlow.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Common
{
    internal class Commanders
    {
        public enum CommanderList {
            Sharpshooter = 1, Amanitore, Artemisia, Gilgamesh,
            HondaTatakatsu, AdmiralYi, AfterTheMajesty, Theodora,
            Xenovia, Aethius, Scipio,
            Dragoon, Hangyu, AlexanderNevsky, WilliamI, MinamotoYoshitsune, Chandragupta, BertrandDuGuettain, YardFee,
            None,
        };
        public static Dictionary<CommanderList, SkillBase> atkeyValuePairs = new Dictionary<CommanderList, SkillBase> { 
            { CommanderList.Sharpshooter, new Markswoman() },
            { CommanderList.Dragoon, new Dragon_Lancer() },
            { CommanderList.Hangyu, new Xiang_Yu() },
            { CommanderList.AlexanderNevsky, new Alexander_Nevsky()},
            { CommanderList.WilliamI, new William()},
            { CommanderList.HondaTatakatsu, new Honda_Tadakatsu()},
            { CommanderList.MinamotoYoshitsune, new Minamoto_no_Yoshitsune()},
            { CommanderList.Chandragupta, new Chandragupta_Maurya()},
            { CommanderList.BertrandDuGuettain, new Bertrand_du_Guesclin()},
            { CommanderList.AdmiralYi, new Yi_Sun_Sin()},
            { CommanderList.YardFee, new Jadwiga()},
            { CommanderList.Xenovia, new Zenobia()},
            { CommanderList.Aethius, new Aetius()},
            { CommanderList.Scipio, new Scipio()},
            { CommanderList.AfterTheMajesty, new Wu_Zetian()},
            { CommanderList.Amanitore, new Amanitore()},
            { CommanderList.Artemisia, new Artemisia()},
            { CommanderList.Theodora, new Theodora()},
            { CommanderList.Gilgamesh, new Gilgamesh()},
            { CommanderList.None, new SkillBase() }
        };
        public static Dictionary<CommanderList, SkillBase> dfkeyValuePairs = new Dictionary<CommanderList, SkillBase> {
            { CommanderList.Sharpshooter, new Markswoman() },
            { CommanderList.Dragoon, new Dragon_Lancer() },
            { CommanderList.Hangyu, new Xiang_Yu() },
            { CommanderList.AlexanderNevsky, new Alexander_Nevsky()},
            { CommanderList.WilliamI, new William()},
            { CommanderList.HondaTatakatsu, new Honda_Tadakatsu()},
            { CommanderList.MinamotoYoshitsune, new Minamoto_no_Yoshitsune()},
            { CommanderList.Chandragupta, new Chandragupta_Maurya()},
            { CommanderList.BertrandDuGuettain, new Bertrand_du_Guesclin()},
            { CommanderList.AdmiralYi, new Yi_Sun_Sin()},
            { CommanderList.YardFee, new Jadwiga()},
            { CommanderList.Xenovia, new Zenobia()},
            { CommanderList.Aethius, new Aetius()},
            { CommanderList.Scipio, new Scipio()},
            { CommanderList.AfterTheMajesty, new Wu_Zetian()},
            { CommanderList.Amanitore, new Amanitore()},
            { CommanderList.Artemisia, new Artemisia()},
            { CommanderList.Theodora, new Theodora()},
            { CommanderList.Gilgamesh, new Gilgamesh()},
            { CommanderList.None, new SkillBase() }
        };


    }
}

