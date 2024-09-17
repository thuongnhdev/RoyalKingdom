using System.Collections;
using System.Collections.Generic;
using CoreData.UniFlow.Common;
using Fbs;
using UnityEngine;

namespace Assets.Dev.Tutorial.Scripts
{
    public class TutorialData : MonoSingleton<TutorialData>
    {
        [SerializeField] public TutorialTracker TutorialTracker;

        public StepNpc StepNpcCurrent = StepNpc.Npc1;
        public StepTutorial StepTutorialCurrent = StepTutorial.TutorialStart;

        public EndBattleModel EndBattleData;

        public readonly float TimeTutorialTroopMove = 1000;
        public readonly float TimeTutorialMoveCameraX = 8000;
        public readonly float TimeMoveCameraNpc1 = 2000;
        public readonly float TimeNpcTalkBalloon1 = 8000;
        public readonly float TimeTutorialTroopMoveAttack = 2000;
        public readonly float TimeTutorialJoinTown = 4000;
        public readonly float TimeTutorialBattle1 = 1000;
        public readonly float TimeTutorialBattleNpc1 = 7000;
        public readonly float TimeStepTroopPlayerMoveGateTown = 2000;
        public readonly float TimeShowNpc2 = 9000;
        public readonly float TimeDelayMoveArcherToBattle = 4000;
        public readonly float TimeShowNpc3 = 3500;
        public readonly float TimeTutorialBattle2 = 4000;
        public readonly float TimeShowNpc4 = 22000;
        public readonly float TimeShowNpc5 = 6000;
        public readonly float TimeShowNpc6 = 6000;
        public readonly float TimeTutorialBattleNpc3 = 12000;
        public readonly float TimeShowTooltipTalk = 2000;
        public readonly float TimeShowTooltipTalkFallBack = 5000;
        public readonly float TimeShowNpc8 = 15000;
        public readonly float TimeTutorialCity = 4000;
        public readonly float TimeMoveBattleCenter = 3000;
        public readonly float TimeDelayMovePlayerAi = 12000;
        public readonly float TimeDelayStartBattleStep2 = 30000;
        public readonly float TimeDelayStartBattleStep2Upgrade = 23000;
        public readonly float TimeDelayChangeRotationTroopAi = 10000;
        public readonly float TimeShowBattleStep3 = 30000;
        public readonly float TimeDelayDoneTutorial = 20000;

        public void Reset()
        {
            StepNpcCurrent = StepNpc.Npc1;
            StepTutorialCurrent = StepTutorial.TutorialStart;
        }

        public enum StepNpc
        {
            Npc1 = 0,
            Npc2 = 1,
            Npc3 = 2,
            Npc4 = 3,
            Npc5 = 4,
            Npc6 = 5,
            Npc7 = 6,
            Npc8 = 7
        }

        public enum StepTutorial
        {
            TutorialStart = 0,
            TutorialTroopMove = 1,
            TutorialCityBandit = 2,
            TutorialTroopTooltipKill = 3,
            TutorialTroopMoveAttack = 4,
            TutorialTroopAttack = 5,
            TutorialTroopWin = 6,
            TutorialCityTooltipJoin = 7,
            TutorialTroopConfirmCity = 8,
            TutorialJoinTown = 9,
            TutorialClickTown = 10,
            TutorialBattleBradit1 = 11,
            TutorialBattleBradit2 = 12,
            TutorialBattleBradit3 = 13,
            TutorialBatleBraditCity = 14,
            TutorialMergeLan = 15,
            TutorialDone = 16,
        }

        public void SetEndBattle(EndBattleModel endBattel)
        {
            EndBattleData = endBattel;
        }

        public EndBattleModel GetEndBattle()
        {
            return EndBattleData;
        }

        public void SetStepNpc(StepNpc step)
        {
            StepNpcCurrent = step;
        }

        public StepNpc GetStepNpc()
        {
            return StepNpcCurrent;
        }

        public void SetStepTutorial(StepTutorial step)
        {
            StepTutorialCurrent = step;
        }

        public StepTutorial GetStepTutorial()
        {
            return StepTutorialCurrent;
        }

    }
}

