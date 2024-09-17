using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoreData.UniFlow.Commander.CommanderBase;
using static CoreData.UniFlow.Common.Character;
using static CoreData.UniFlow.Common.Commanders;
using static CoreData.UniFlow.Common.Equipment;
using static CoreData.UniFlow.Common.Tiers;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using CoreData.UniFlow.Tier;

namespace CoreData.UniFlow.Common
{
    internal static class SelectInfo
    {
        public static void SetIsField(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
               Debug.LogFormat("1. Field");
               Debug.LogFormat("2. Gathering");
               Debug.LogFormat("3. Garrison");
                string str = (sr == null ? Console.ReadLine() : sr.ReadLine());
                int num;
                if (int.TryParse(str, out num) == true)
                {
                    if (num != 1 && num != 2 && num != 3)
                    {
                        Debug.LogFormat("Please enter the correct number.");
                        continue;
                    }
                }
                else
                {
                    if (str != "Field" && str != "Gathering" && str != "Garrison")
                    {
                        Debug.LogFormat("Please enter the correct name.");
                        continue;
                    }
                }
                Debug.LogFormat("{0}You have selected.", num == 0? str : num == 1? "Field" : (num == 2? "Gathering" : "Garrison"));
            
                BattleState battleState = (num == 0? (str == "Field" ? BattleState.Field : (str == "Gathering" ? BattleState.Conquering : BattleState.Garrison)) :(num == 1 ? BattleState.Field : (num == 2 ? BattleState.Conquering : BattleState.Garrison)));
                commander.battleState = battleState;
                break;
                
            }
        }
        public static void SetCommander(CommanderBase commander, bool isFirst, bool isAttacker, StreamReader sr)
        {
            while (true)
            {
                foreach (CommanderList data in Enum.GetValues(typeof(CommanderList)))
                {
                    Debug.LogFormat((int)data + ". " + data);
                }
                string str = (sr == null? Console.ReadLine() : sr.ReadLine());
                int num;
                if (int.TryParse(str, out num) == true)
                {
                    if (Enum.IsDefined(typeof(CommanderList), num) == false)
                    {
                        Debug.LogFormat("Please enter the correct number.");
                        continue;
                    }
                }
                else
                {
                    if (Enum.IsDefined(typeof(CommanderList), str) == false)
                    {
                        Debug.LogFormat("Please enter the correct name.");
                        continue;
                    }
                }
                var enumCommander = Enum.Parse(typeof(CommanderList), str);
                Debug.LogFormat("The selected commander is {0}.", num == 0? str : Enum.GetName(typeof(CommanderList), num));
            

                if (isAttacker)
                {
                    commander.site = "offensive side";
                    Commanders.atkeyValuePairs[(CommanderList)enumCommander].Init(commander, isFirst);
                    commander.commanderClassList.Add((Commanders.atkeyValuePairs[(CommanderList)enumCommander], isFirst));
                }
                else
                {
                    commander.site = "tossing";
                    Commanders.dfkeyValuePairs[(CommanderList)enumCommander].Init(commander, isFirst);
                    commander.commanderClassList.Add((Commanders.dfkeyValuePairs[(CommanderList)enumCommander], isFirst));
                }
                break;
            }
        }
        public static void SetTroopType(CommanderBase commander, bool isAttacker, StreamReader sr)
        {
            while (true)
            {
                foreach (TierList data in Enum.GetValues(typeof(TierList)))
                {
                    Debug.Log((int)data + ". " + data);
                }
                string str = (sr == null ? Console.ReadLine() : sr.ReadLine());
                int num;
                if (int.TryParse(str, out num) == true)
                {
                    if (Enum.IsDefined(typeof(TierList), num) == false)
                    {
                       Debug.LogFormat("Please enter the correct number.");
                        continue;
                    }
                }
                else
                {
                    if (Enum.IsDefined(typeof(TierList), str) == false)
                    {
                        Debug.Log("Please enter the correct name.");
                        continue;
                    }
                }
                var enumTier = Enum.Parse(typeof(TierList), str);
                Debug.LogFormat("The selected disease type is {0}.", num == 0 ? str : Enum.GetName(typeof(TierList), num));
          
                if (isAttacker)
                    Tiers.atkeyValuePairs[(TierList)enumTier].Init(commander);
                else
                    Tiers.dfkeyValuePairs[(TierList)enumTier].Init(commander);
                break;
            }
        }
        public static void SetTroopAmount(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                int stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (int.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The number of units entered is {0}.", stat);
                  
                    commander.maxTroop = stat;
                    commander.troop = stat;
                    break;
                }
            }
        }
        public static void SetAdditionalAttack(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The additional attack power you entered is {0}.", stat);
                   
                    commander.additionalAttack += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalDefence(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The additional defense you entered is {0}.", stat);
                    
                    commander.additionalDefence += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalHealth(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The additional life you entered is {0}.", stat);
                
                    commander.additionalHealth += stat;
                    break;
                }
            }
        }

        public static void SetAdditionalDamageIncrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("All damage entered is {0}.", stat);
                   
                    commander.additionalDamageIncrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalNormalDamageIncrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The normal attack damage you entered is {0}.", stat);
                   
                    commander.additionalNormalDamageIncrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalCounterDamageIncrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The counterattack damage you entered is {0}.", stat);
               
                    commander.additionalCounterDamageIncrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalSkillDamageIncrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                   Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                   Debug.LogFormat("The skill damage you entered is {0}.", stat);
              
                    commander.additionalSkillDamageIncrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalDamageDecrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                   Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                   Debug.LogFormat("All damage reduction you entered is {0}.", stat);
              
                    commander.additionalDamageDecrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalNormalDamageDecrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                   Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                   Debug.LogFormat("The normal damage reduction you entered is {0}.", stat);
       
                    commander.additionalNormalDamageDecrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalCounterDamageDecrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                   Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                   Debug.LogFormat("The counterattack damage reduction you entered is {0}.", stat);
          
                    commander.additionalCounterDamageDecrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalSkillDamageDecrease(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                   Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                   Debug.LogFormat("The skill damage reduction you entered is {0}.", stat);

                    commander.additionalSkillDamageDecrease += stat;
                    break;
                }
            }
        }
        public static void SetAdditionalHealingEffect(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                double stat;
                string statString = (sr == null ? Console.ReadLine() : sr.ReadLine());
                if (double.TryParse(statString, out stat) == false)
                {
                   Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                   Debug.LogFormat("The treatment effect increase you entered is {0}.", stat);
           
                    commander.additionalHealingEffect += stat;
                    break;
                }
            }
        }


        public static void SetCharacter(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                foreach (CharacterList data in Enum.GetValues(typeof(CharacterList)))
                {
                   Debug.LogFormat((int)data + ". " + data);
                }
                string st = (sr == null? Console.ReadLine() : sr.ReadLine());
                if (string.IsNullOrEmpty(st))
                    return;

                st = st.Replace(" ", "");
                string[] stList = st.Split(',');
                if (stList.Length != 3)
                {
                   Debug.LogFormat("Please select 3 characteristics.");
                }
                bool success = true;
                foreach (string ch in stList)
                {
                    int num = int.Parse(ch);
                    if (Enum.IsDefined(typeof(CharacterList), num) == false)
                    {
                       Debug.LogFormat("Please enter the correct number.");
                        success = false;
                        break;
                    }
                }
                if (success == true)
                {
                    foreach (string ch in stList)
                    {
                        int num = int.Parse(ch);
                        var enumCharacter = Enum.Parse(typeof(CharacterList), ch);
                        Console.Write("{0} ", Enum.GetName(typeof(CharacterList), num));
                        commander.commanderCharacter.Add((CharacterList)enumCharacter);
                    }   
                   Debug.LogFormat("You have entered a characteristic.");
              
                    break;
                }
            }
        }
        public static void SetCharacterSize(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                string st = (sr == null ? Console.ReadLine() : sr.ReadLine());
                if (string.IsNullOrEmpty(st))
                    return;

                st = st.Replace(" ", "");

                string[] stList = st.Split(',');
                foreach (string ch in stList)
                {
                    string[] numCount = ch.Split('-');
                    if (numCount.Length == 1)
                        continue;

                    int num = int.Parse(numCount[0]);
                    int count = int.Parse(numCount[1]);
                    var enumCharacter = Enum.Parse(typeof(CharacterList), numCount[0]);
                    Console.Write("{0} ", Enum.GetName(typeof(CharacterList), num));
                    commander.characterList.Add(((CharacterList)enumCharacter, count));
                }
                break;
                Debug.LogFormat("You have entered.");
            }
        }

        public static void SetSubCharacter(CommanderBase commander, bool isAttacker, StreamReader sr)
        {
        
        }

        public static void SetEquipment(CommanderBase commander, bool isAttacker, StreamReader sr)
        {
            while (true)
            {
                foreach (EquipmentList data in Enum.GetValues(typeof(EquipmentList)))
                {
                   Debug.LogFormat((int)data + ". " + data);
                }
                string st = (sr == null? Console.ReadLine() : sr.ReadLine());
                st = st.Replace(" ", "");
                string[] stList = st.Split(',');
                foreach (string ch in stList)
                {
                    int num = int.Parse(ch);
                    if (Enum.IsDefined(typeof(EquipmentList), num) == false)
                    {
                       Debug.LogFormat("Please enter the correct number.");
                    }
                    else
                    {
                        var enumEquipment = Enum.Parse(typeof(EquipmentList), ch);
                       Debug.LogFormat("The selected device is {0}.", Enum.GetName(typeof(EquipmentList), num));
                   
                        if (isAttacker)
                        {
                            Equipment.atkeyValuePairs[(EquipmentList)enumEquipment].Init(commander, (num % 2 == 0 ? true : false));
                            commander.equipmentClassList.Add((Equipment.atkeyValuePairs[(EquipmentList)enumEquipment], (num % 2 == 0 ? true : false)));
                        }
                        else
                        {
                            Equipment.dfkeyValuePairs[(EquipmentList)enumEquipment].Init(commander, (num % 2 == 0 ? true : false));
                            commander.equipmentClassList.Add((Equipment.dfkeyValuePairs[(EquipmentList)enumEquipment], (num % 2 == 0 ? true : false)));
                        }
                    }
                }
                break;
            }
        }
        public static void SetCoinAmount(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                int stat;
                string statString = (sr == null ? Console.ReadLine() : sr.ReadLine());
                if (int.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The number of units entered is {0}.", stat);

                    commander.coin = stat;
                    break;
                }
            }
        }

        public static void SetWoodAmount(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                int stat;
                string statString = (sr == null ? Console.ReadLine() : sr.ReadLine());
                if (int.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The number of units entered is {0}.", stat);

                    commander.wood = stat;
                    break;
                }
            }
        }

        public static void SetStoneAmount(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                int stat;
                string statString = (sr == null ? Console.ReadLine() : sr.ReadLine());
                if (int.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The number of units entered is {0}.", stat);

                    commander.stone = stat;
                    break;
                }
            }
        }

        public static void SetWaterAmount(CommanderBase commander, StreamReader sr)
        {
            while (true)
            {
                int stat;
                string statString = (sr == null ? Console.ReadLine() : sr.ReadLine());
                if (int.TryParse(statString, out stat) == false)
                {
                    Debug.LogFormat("Please enter the correct number.");
                }
                else
                {
                    Debug.LogFormat("The number of units entered is {0}.", stat);

                    commander.water = stat;
                    break;
                }
            }
        }

        public static void SetRunType(CommanderBase at, CommanderBase df, StreamReader sr)
        {
          
        }

        public static void DirectSetting(string txtName , Action<CommanderBase> onComplete)
        {
            string path = "Assets/Resources/SampleData/Comprehensivedata/" + txtName;
            StreamReader sr = string.IsNullOrEmpty(txtName)? null : new StreamReader(path);

            CommanderBase commander_at = new CommanderBase();
            CommanderBase commander_df = new CommanderBase();

            SetIsField(commander_at, sr);
            SetCommander(commander_at, true, true, sr);
            SetCommander(commander_at, false, true, sr);
            SetTroopType(commander_at, true, sr);
            SetTroopAmount(commander_at, sr);
            SetAdditionalAttack(commander_at, sr);
            SetAdditionalDefence(commander_at, sr);
            SetAdditionalHealth(commander_at, sr);
            SetAdditionalDamageIncrease(commander_at, sr);
            SetAdditionalNormalDamageIncrease(commander_at, sr);
            SetAdditionalCounterDamageIncrease(commander_at, sr);
            SetAdditionalSkillDamageIncrease(commander_at, sr);
            SetAdditionalDamageDecrease(commander_at, sr);
            SetAdditionalNormalDamageDecrease(commander_at, sr);
            SetAdditionalCounterDamageDecrease(commander_at, sr);
            SetAdditionalSkillDamageDecrease(commander_at, sr);
            SetAdditionalHealingEffect(commander_at, sr);
            SetCharacter(commander_at, sr);
            SetCharacterSize(commander_at, sr);
            //SetSubCharacter(commander_at, true, sr);
            SetEquipment(commander_at, true, sr);
            SetCoinAmount(commander_at, sr);
            SetWoodAmount(commander_at, sr);
            SetStoneAmount(commander_at, sr);
            SetWaterAmount(commander_at, sr);
            onComplete?.Invoke(commander_at);

            _commander = commander_at;
        }

        public static void AddCharacter(CommanderBase commander, CharacterList enumCharacter, int number)
        {
            var item = commander.characterList.Find(t => (CharacterList)t.Item1 == (CharacterList)enumCharacter);
            if (item.Item1 == null)
                commander.characterList.Add(((CharacterList)enumCharacter, number));
            else
            {
                commander.characterList.Remove(item);
                int newSize = number + item.Item2;
                commander.characterList.Add(((CharacterList)enumCharacter, newSize));
            }
        }

        private static CommanderBase _commander;
        public static CommanderBase CommanderBase
        {
            get { return _commander; }
            set { _commander = value; }
        }
    }
}
