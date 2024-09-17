using CoreData.UniFlow.Characteristic;
using CoreData.UniFlow.Tier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Common
{
    public class Character
    {
        public enum CharacterList
        {
            Vassar, Farmer , AssistantWorker, Clown, Priest, Tinker, Poacher, Apothecary, Scholars, Hashslinger, Painter, Warrior, Official
        };

        public enum Mood { Bad, Normal, Good }
        public enum Gender { Male, Female }
        public enum HealthStatus { Bad, Normal, Good }
        public enum AbilityStats { Leadership, Intelligence, Wisdom, Strength, PhysicalStrength, Finesse, Charm, Speech, Sophistication, Analysis, Sociability, Patience, Luck, Faith, Karma }
       
        #region top
        public enum JobVassar { Normal, Work, Action }
        public enum JobFarmer { Fisherman, Miner, WoodCutter, Miller, Barber, Swineherd, Shepherd, Adanist, Steward, Beekeeper, Groom, Avener, GardenMaster }
        public enum JobMerchant { PawnBroker, Peddlar, Mercer, HayMerchant, IronMonger, Usurer, Dealer, Grocer, Banker, Accountant }
        public enum JobClown { Acrobat, Dancers, Bard, FortuneTellers, Comedians, Poet, Mimes, Soothsayers, Impresario, Astrologer }
        public enum JobPriest { Pilgrims, Friar, Monk, Preacher, Theologian, Pardoner, Oracles, Leech, Curate, Almoner, Abbot, Bishop, Cardinal, Chaplain, Archbishop, Pope }
        public enum JobTinker { Dyer, Artisan, Repairman, Spinner, Blacksmith, Carpenter, Plumber, Glassblower, Shepster, Fletcher, Lapidary, Fullers, Goldsmith, Artillerist, Craftsman, Siegecrafter , Shipwright }
        public enum JobPoacher { Mercenary, Thief, Bandit, Pirate, Condottiere, BanditBoss, PirateBoss, Physician }
        public enum JobApothecary { Surgeon, Physician }
        public enum JobScholars { Mathematician, Linguist, Scientist, Professor, Diplomat, Translator, Seneschal, Philosophy, Archaeologist, Alchemists, Judge, Emissary, Prefect, Scribe, Astronomer, Sheriff, Ambassador, Financier, LordHighSteward, LordHighChancellor, LordHighTreasurer }
        public enum JobHashslinger { Pastrycook, Vintner, Almoner, Baker, Brewer, HeadChef, RoyalChef }
        public enum JobPainter { Sculptor, Artist }
        public enum JobWarrior { Mate, Bodyguard, Drummer, Falconer, Squire, Scount, Pilot, HouseCarl, Praetor, Spy, Knight, Explorer, Skipper, Marshal, Consul, Surveyor, Admiral, Constable, Banneret, TacticianMaster, Imperator }
        public enum JobOfficial { Warder, Librarian, Supplier, Janitor, Bailiff, LordChamberlain }

        #endregion

        public static Dictionary<CharacterList, Enum> CharacterSubCharacterPairs = new Dictionary<CharacterList, Enum> {
            { CharacterList.Vassar, new JobVassar() },
            { CharacterList.Farmer, new JobFarmer() },
            { CharacterList.AssistantWorker, new JobMerchant() },
            { CharacterList.Clown, new JobClown() },
            { CharacterList.Priest, new JobPriest() },

            { CharacterList.Tinker, new JobTinker() },
            { CharacterList.Poacher, new JobPoacher() },
            { CharacterList.Apothecary, new JobApothecary() },
            { CharacterList.Scholars, new JobScholars() },
            { CharacterList.Hashslinger, new JobHashslinger() },
            { CharacterList.Painter, new JobPainter() },

            { CharacterList.Warrior, new JobWarrior() },
            { CharacterList.Official, new JobOfficial() },
        };

        public static Dictionary<Enum, CharacterBase> atkeyValuePairs = new Dictionary<Enum, CharacterBase> {
            #region top
            { JobVassar.Normal, new Characteristic.Vassar.Vassar_Normal() },
            { JobVassar.Work, new Characteristic.Vassar.Vassar_Work() },
            { JobVassar.Action, new Characteristic.Vassar.Vassar_Action() },

            { JobFarmer.Fisherman, new Characteristic.Farmer.Farmer_Fisherman() },
            { JobFarmer.Miner, new Characteristic.Farmer.Farmer_Miner() },
            { JobFarmer.WoodCutter, new Characteristic.Farmer.Farmer_WoodCutter() },
            { JobFarmer.Miller, new Characteristic.Farmer.Farmer_Miller() },
            { JobFarmer.Barber, new Characteristic.Farmer.Farmer_Barber() },
            { JobFarmer.Swineherd, new Characteristic.Farmer.Farmer_Swineherd() },
            { JobFarmer.Beekeeper, new Characteristic.Farmer.Farmer_Beekeeper() },
            { JobFarmer.Groom, new Characteristic.Farmer.Farmer_Groom() },
            { JobFarmer.Avener, new Characteristic.Farmer.Farmer_Avener() },
            { JobFarmer.GardenMaster, new Characteristic.Farmer.Farmer_GardenMaster() },

            { JobMerchant.PawnBroker, new Characteristic.Merchant.Merchant_PawnBroker() },
            { JobMerchant.Peddlar, new Characteristic.Merchant.Merchant_Peddlar() },
            { JobMerchant.Mercer, new Characteristic.Merchant.Merchant_Mercer() },
            { JobMerchant.HayMerchant, new Characteristic.Merchant.Merchant_HayMerchant() },
            { JobMerchant.IronMonger, new Characteristic.Merchant.Merchant_IronMonger() },
            { JobMerchant.Usurer, new Characteristic.Merchant.Merchant_Usurer() },
            { JobMerchant.Dealer, new Characteristic.Merchant.Merchant_Dealer() },
            { JobMerchant.Grocer, new Characteristic.Merchant.Merchant_Grocer() },
            { JobMerchant.Banker, new Characteristic.Merchant.Merchant_Banker() },
            { JobMerchant.Accountant, new Characteristic.Merchant.Merchant_Accountant() },

            { JobClown.Acrobat, new Characteristic.Clown.Clown_Acrobat() },
            { JobClown.Dancers, new Characteristic.Clown.Clown_Dancers() },
            { JobClown.Bard, new Characteristic.Clown.Clown_Bard() },
            { JobClown.FortuneTellers, new Characteristic.Clown.Clown_FortuneTellers() },
            { JobClown.Comedians, new Characteristic.Clown.Clown_Comedians() },
            { JobClown.Poet, new Characteristic.Clown.Clown_Poet() },
            { JobClown.Mimes, new Characteristic.Clown.Clown_Mimes() },
            { JobClown.Soothsayers, new Characteristic.Clown.Clown_Soothsayers() },
            { JobClown.Impresario, new Characteristic.Clown.Clown_Impresario() },
            { JobClown.Astrologer, new Characteristic.Clown.Clown_Astrologer() },

            { JobPriest.Pilgrims, new Characteristic.Priest.Priest_Pilgrims() },
            { JobPriest.Friar, new Characteristic.Priest.Priest_Friar() },
            { JobPriest.Monk, new Characteristic.Priest.Priest_Monk() },
            { JobPriest.Preacher, new Characteristic.Priest.Priest_Preacher() },
            { JobPriest.Theologian, new Characteristic.Priest.Priest_Theologian() },
            { JobPriest.Pardoner, new Characteristic.Priest.Priest_Pardoner() },
            { JobPriest.Oracles, new Characteristic.Priest.Priest_Oracles() },
            { JobPriest.Leech, new Characteristic.Priest.Priest_Leech() },
            { JobPriest.Curate, new Characteristic.Priest.Priest_Curate() },
            { JobPriest.Almoner, new Characteristic.Priest.Priest_Almoner() },
            { JobPriest.Abbot, new Characteristic.Priest.Priest_Abbot() },
            { JobPriest.Bishop, new Characteristic.Priest.Priest_Bishop() },
            { JobPriest.Cardinal, new Characteristic.Priest.Priest_Cardinal() },
            { JobPriest.Chaplain, new Characteristic.Priest.Priest_Chaplain() },
            { JobPriest.Archbishop, new Characteristic.Priest.Priest_Archbishop() },
            { JobPriest.Pope, new Characteristic.Priest.Priest_Pope() },

            { JobTinker.Dyer, new Characteristic.Tinker.Tinker_Dyer() },
            { JobTinker.Artisan, new Characteristic.Tinker.Tinker_Artisan() },
            { JobTinker.Repairman, new Characteristic.Tinker.Tinker_Repairman() },
            { JobTinker.Spinner, new Characteristic.Tinker.Tinker_Spinner() },
            { JobTinker.Blacksmith, new Characteristic.Tinker.Tinker_Blacksmith() },
            { JobTinker.Carpenter, new Characteristic.Tinker.Tinker_Carpenter() },
            { JobTinker.Plumber, new Characteristic.Tinker.Tinker_Plumber() },
            { JobTinker.Glassblower, new Characteristic.Tinker.Tinker_Glassblower() },
            { JobTinker.Shepster, new Characteristic.Tinker.Tinker_Shepster() },
            { JobTinker.Fletcher, new Characteristic.Tinker.Tinker_Fletcher() },
            { JobTinker.Lapidary, new Characteristic.Tinker.Tinker_Lapidary() },
            { JobTinker.Fullers, new Characteristic.Tinker.Tinker_Fullers() },
            { JobTinker.Goldsmith, new Characteristic.Tinker.TInker_Goldsmith() },
            { JobTinker.Artillerist, new Characteristic.Tinker.Tinker_Artillerist() },
            { JobTinker.Craftsman, new Characteristic.Tinker.Tinker_Craftsman() },
            { JobTinker.Siegecrafter, new Characteristic.Tinker.Tinker_Siegecrafter() },
            { JobTinker.Shipwright, new Characteristic.Tinker.Tinker_Shipwright() },

            { JobPoacher.Mercenary, new Characteristic.Poacher.Poacher_Mercenary() },
            { JobPoacher.Thief, new Characteristic.Poacher.Poacher_Thief() },
            { JobPoacher.Bandit, new Characteristic.Poacher.Poacher_Bandit() },
            { JobPoacher.Pirate, new Characteristic.Poacher.Poacher_Pirate() },
            { JobPoacher.Condottiere, new Characteristic.Poacher.Poacher_Condottiere() },
            { JobPoacher.BanditBoss, new Characteristic.Poacher.Poacher_BanditBoss() },
            { JobPoacher.PirateBoss, new Characteristic.Poacher.Poacher_PirateBoss() },

            { JobApothecary.Surgeon, new Characteristic.Apothecary.Apothecary_Surgeon() },
            { JobApothecary.Physician, new Characteristic.Apothecary.Apothecary_Physician() },

            { JobScholars.Mathematician, new Characteristic.Scholars.Scholars_Mathematician() },
            { JobScholars.Linguist, new Characteristic.Scholars.Scholars_Linguist() },
            { JobScholars.Scientist, new Characteristic.Scholars.Scholars_Scientist() },
            { JobScholars.Professor, new Characteristic.Scholars.Scholars_Professor() },
            { JobScholars.Diplomat, new Characteristic.Scholars.Scholars_Diplomat() },
            { JobScholars.Translator, new Characteristic.Scholars.Scholars_Translator() },
            { JobScholars.Seneschal, new Characteristic.Scholars.Scholars_Seneschal() },
            { JobScholars.Philosophy, new Characteristic.Scholars.Scholars_Philosophy() },
            { JobScholars.Archaeologist, new Characteristic.Scholars.Scholars_Archaeologist() },
            { JobScholars.Alchemists, new Characteristic.Scholars.Scholars_Alchemists() },
            { JobScholars.Judge, new Characteristic.Scholars.Scholars_Judge() },
            { JobScholars.Emissary, new Characteristic.Scholars.Scholars_Emissary() },
            { JobScholars.Prefect, new Characteristic.Scholars.Scholars_Prefect() },
            { JobScholars.Scribe, new Characteristic.Scholars.Scholars_Scribe() },
            { JobScholars.Astronomer, new Characteristic.Scholars.Scholars_Astronomer() },
            { JobScholars.Sheriff, new Characteristic.Scholars.Scholars_Sheriff() },
            { JobScholars.Ambassador, new Characteristic.Scholars.Scholars_Ambassador() },
            { JobScholars.Financier, new Characteristic.Scholars.Scholars_Financier() },
            { JobScholars.LordHighSteward, new Characteristic.Scholars.Scholars_LordHighSteward() },
            { JobScholars.LordHighChancellor, new Characteristic.Scholars.Scholars_LordHighChancellor() },
            { JobScholars.LordHighTreasurer, new Characteristic.Scholars.Scholars_LordHighTreasurer() },

            { JobHashslinger.Pastrycook, new Characteristic.Hashslinger.Hashslinger_Pastrycook() },
            { JobHashslinger.Vintner, new Characteristic.Hashslinger.Hashslinger_Vintner() },
            { JobHashslinger.Almoner, new Characteristic.Hashslinger.Hashslinger_Almoner() },
            { JobHashslinger.Baker, new Characteristic.Hashslinger.Hashslinger_Baker() },
            { JobHashslinger.Brewer, new Characteristic.Hashslinger.Hashslinger_Brewer() },
            { JobHashslinger.HeadChef, new Characteristic.Hashslinger.Hashslinger_HeadChef() },
            { JobHashslinger.RoyalChef, new Characteristic.Hashslinger.Hashslinger_RoyalChef() },

            { JobPainter.Sculptor, new Characteristic.Painter.Painter_Sculptor() },
            { JobPainter.Artist, new Characteristic.Painter.Painter_Artist() },

            { JobWarrior.Mate, new Characteristic.Warrior.Warrior_Mate() },
            { JobWarrior.Bodyguard, new Characteristic.Warrior.Warrior_Bodyguard() },
            { JobWarrior.Drummer, new Characteristic.Warrior.Warrior_Drummer() },
            { JobWarrior.Falconer, new Characteristic.Warrior.Warrior_Falconer() },
            { JobWarrior.Squire, new Characteristic.Warrior.Warrior_Squire() },
            { JobWarrior.Scount, new Characteristic.Warrior.Warrior_Scount() },
            { JobWarrior.Pilot, new Characteristic.Warrior.Warrior_Pilot() },
            { JobWarrior.HouseCarl, new Characteristic.Warrior.Warrior_HouseCarl() },
            { JobWarrior.Praetor, new Characteristic.Warrior.Warrior_Praetor() },
            { JobWarrior.Spy, new Characteristic.Warrior.Warrior_Spy() },
            { JobWarrior.Knight, new Characteristic.Warrior.Warrior_Knight() },
            { JobWarrior.Explorer, new Characteristic.Warrior.Warrior_Explorer() },
            { JobWarrior.Skipper, new Characteristic.Warrior.Warrior_Skipper() },
            { JobWarrior.Marshal, new Characteristic.Warrior.Warrior_Marshal() },
            { JobWarrior.Consul, new Characteristic.Warrior.Warrior_Consul() },
            { JobWarrior.Surveyor, new Characteristic.Warrior.Warrior_Surveyor() },
            { JobWarrior.Admiral, new Characteristic.Warrior.Warrior_Admiral() },
            { JobWarrior.Constable, new Characteristic.Warrior.Warrior_Constable() },
            { JobWarrior.Banneret, new Characteristic.Warrior.Warrior_Banneret() },
            { JobWarrior.TacticianMaster, new Characteristic.Warrior.Warrior_TacticianMaster() },
            { JobWarrior.Imperator, new Characteristic.Warrior.Warrior_Imperator() },

            { JobOfficial.Warder, new Characteristic.Official.Official_Warder() },
            { JobOfficial.Librarian, new Characteristic.Official.Official_Librarian() },
            { JobOfficial.Supplier, new Characteristic.Official.Official_Supplier() },
            { JobOfficial.Janitor, new Characteristic.Official.Official_Janitor() },
            { JobOfficial.Bailiff, new Characteristic.Official.Official_Bailiff() },
            { JobOfficial.LordChamberlain, new Characteristic.Official.Official_LordChamberlain() },
            #endregion
          
        };
  
    }
}
