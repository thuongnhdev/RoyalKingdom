using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Dev.Tutorial.Scripts;
using CoreData.UniFlow.Common;
using JetBrains.Annotations;
using UnityEngine;

public class TutorialBattle : MonoSingleton<TutorialBattle>
{
    public enum StepBattleTutorial
    {
        Step1 = 1,
        Step2 = 2,
        Step2Upgrade = 3,
        Step3 = 4,
        Done = 5
    }

    public void StepBattle(StepBattleTutorial stepBattleTutorial)
    {
        switch (stepBattleTutorial)
        {
            case StepBattleTutorial.Step1:
            {
                WorldMapTroopModel modeMe = TutorialWorldmap.Instance.GetTroopCavalryMeTutorial();
                WorldMapTroopModel modelBradit1 = TutorialWorldmap.Instance.GetTroopEnemyTutorial();
                StepBattle_1(modeMe, modelBradit1);
            }
                break;
            case StepBattleTutorial.Step2:
            {
                WorldMapTroopModel modeCavalryMe = TutorialWorldmap.Instance.GetTroopCavalryMeTutorial();
                WorldMapTroopModel modelBradit2 = TutorialWorldmap.Instance.GetTroopEnemyAITutorial(1);
                WorldMapTroopModel modelBradit3 = TutorialWorldmap.Instance.GetTroopEnemyAITutorial(2);
                StepBattle_2(modeCavalryMe, modelBradit2, modelBradit3);
            }
                break;
            case StepBattleTutorial.Step3:
            {
                WorldMapTroopModel modelBradit2 = TutorialWorldmap.Instance.GetTroopEnemyAITutorial(1);
                WorldMapTroopModel modelBradit3 = TutorialWorldmap.Instance.GetTroopEnemyAITutorial(2);
                WorldMapTroopModel modelAi2 = TutorialWorldmap.Instance.GetTroopAIMeTutorial(1);
                WorldMapTroopModel modelAi3 = TutorialWorldmap.Instance.GetTroopAIMeTutorial(2);
                StepBattle_3(modelBradit2, modelBradit3, modelAi2, modelAi3);
            }
                break;
            case StepBattleTutorial.Done:
                StopAllCoroutines();
                break;

        }
    }

    public void StepBattle_1(WorldMapTroopModel modelMe, WorldMapTroopModel modelBradit1)
    {
        StartCoroutine(UpdatePawsStep1(modelMe, modelBradit1));
    }

    public void StepBattle_2(WorldMapTroopModel modelMe, WorldMapTroopModel modelBradit2,
        WorldMapTroopModel modelBradit3)
    {
        StopCoroutine("UpdatePawsStep1");
        StartCoroutine(UpdatePawsStep2(modelMe, modelBradit2, modelBradit3));
    }
    
    public void StepBattle_3(WorldMapTroopModel modelBradit2, WorldMapTroopModel modelBradit3,
        WorldMapTroopModel modelAi2, WorldMapTroopModel modelAi3)
    {
        StopCoroutine("UpdatePawsStep2");
        StopCoroutine("UpdatePawsStep2Upgrade");
        StartCoroutine(UpdatePawsStep3(modelBradit2, modelBradit3, modelAi2, modelAi3));
    }
    
    public IEnumerator UpdatePawsStep1(WorldMapTroopModel modelMe, WorldMapTroopModel modelBradit1)
    {
        yield return new WaitForSeconds(5);
        if (modelBradit1.pawns.Count > 0)
            modelMe.UpdatePawsDie(1);
        modelBradit1.UpdatePawsDie(2);
        StartCoroutine(UpdatePawsStep1(modelMe, modelBradit1));
    }

    public IEnumerator UpdatePawsStep2(WorldMapTroopModel modelMe, WorldMapTroopModel modelBradit2,
        WorldMapTroopModel modelBradit3)
    {
        yield return new WaitForSeconds(2.5f);
        modelMe.UpdatePawsDie(1);
        int random = UnityEngine.Random.Range(0, 2);
        if (random == 0)
            modelBradit2.UpdatePawsDie(1);
        else
            modelBradit3.UpdatePawsDie(1);
        StartCoroutine(UpdatePawsStep2(modelMe, modelBradit2, modelBradit3));
    }
    
    public IEnumerator UpdatePawsStep3(WorldMapTroopModel modelBradit2, WorldMapTroopModel modelBradit3,
        WorldMapTroopModel modelAi2, WorldMapTroopModel modelAi3)
    {
        yield return new WaitForSeconds(1);
        modelBradit2.UpdatePawsDie(2);
        modelBradit3.UpdatePawsDie(2);
        int random = UnityEngine.Random.Range(0, 2);
        if (random == 0)
            modelAi2.UpdatePawsDie(1);
        else
            modelAi2.UpdatePawsDie(1);
        StartCoroutine(UpdatePawsStep3(modelBradit2, modelBradit3, modelAi2, modelAi3));
    }
}
