using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    [System.Serializable]
    public class TroopData
    {
        public int IdTroop;
        public VassalSlot Vassal;
        public List<BaseBandDefaut> Band;
        public int Pawns;
        public BuffData Buff;
        public int IdType;
        public TroopData(int idType ,int idTroop,VassalSlot vassal, List<BaseBandDefaut> band,int pawns, BuffData buff)
        {
            IdTroop = idTroop;
            Vassal = vassal;
            Band = band;
            Pawns = pawns;
            Buff = buff;
            IdType = idType;
        }
    }

    [System.Serializable]
    public class VassalSlot
    {
        public int IdVassal_1;
        public int IdVassal_2;
        public int IdVassal_3;
        public int Percent;
        public int Strong;
        public int Fire;
        public int Balance;
        public int Attack;

        public VassalSlot(int idVassal1, int idVassal2, int idVassal3, int percent,int strong,int fire,int balance,int attack)
        {
            IdVassal_1 = idVassal1;
            IdVassal_2 = idVassal2;
            IdVassal_3 = idVassal3;
            Percent = percent;
            Strong = strong;
            Fire = fire;
            Balance = balance;
            Attack = attack;
        }
    }

    [System.Serializable]
    public class BuffData
    {
        public int IsActive;
        public int Buff_1;
        public int Buff_2;
        public int Buff_3;
        public int Buff_4;
        public BuffData(int isActive, int buff_1, int buff_2, int buff_3, int buff_4)
        {
            IsActive = isActive;
            Buff_1 = buff_1;
            Buff_2 = buff_2;
            Buff_3 = buff_3;
            Buff_4 = buff_4;
        }
    }

    [System.Serializable]
    public class TroopDataMax
    {
        public int IdMilitary;
        public int Number;
        public TroopDataMax(int idMilitary, int number)
        {
            IdMilitary = idMilitary;
            Number = number;
        }
    }

    [System.Serializable]
    public enum MilitaryType
    {
        Infantry = 0,
        Archer = 1,
        Cavalry = 2
    }
}
