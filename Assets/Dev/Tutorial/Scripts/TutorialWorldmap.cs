using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData.UniFlow.Common;
using UnityEngine;

namespace Assets.Dev.Tutorial.Scripts
{
    public class TutorialWorldmap : MonoSingleton<TutorialWorldmap>
    {
        public WorldMapControllerV3 wm;

        public UserProfile userProfile;
        
        private TutorialData _tutorialData;

        private TutorialBattle _tutorialBattle;

        private MilitaryManager _militaryManager;

        public WorldMapTroopService troopService;

        private TutorialNpc TutorialNpc;
        
        [SerializeField]
        private GameEvent _onMoveCamera;

        [SerializeField]
        private GameEvent _onZoomCamera;

        public TableCityViewModel tableCityModels;

        [SerializeField] private GameEvent _onFinishShowText;
        [SerializeField] private GameEvent _onClickShowNextTepTutorial;
        public TutorialData GetTutorialData()
        {
            if (_tutorialData == null)
                _tutorialData = TutorialData.Instance;
            return _tutorialData;
        }

        public TutorialBattle GetTutorialBattle()
        {
            if (_tutorialBattle == null)
                _tutorialBattle = TutorialBattle.Instance;
            return _tutorialBattle;
        }

        private void OnEnable()
        {
            _onMoveCamera.Subcribe(OnMoveCamera);
            _onZoomCamera.Subcribe(OnZoomCamera);
            _onFinishShowText.Subcribe(OnFinishShowText);
            _onClickShowNextTepTutorial.Subcribe(OnClickShowNextTepTutorialEvt);
        }

        private void OnDisable()
        {
            _onMoveCamera.Unsubcribe(OnMoveCamera);
            _onZoomCamera.Unsubcribe(OnZoomCamera);
            _onFinishShowText.Unsubcribe(OnFinishShowText);
            _onClickShowNextTepTutorial.Unsubcribe(OnClickShowNextTepTutorialEvt);
        }

        public TutorialNpc GetTutorialNpc()
        {
            if (TutorialNpc == null)
                TutorialNpc = wm.TutorialNpc;
            return TutorialNpc;
        }

        void Start()
        {
            _tutorialData = TutorialData.Instance;
            _tutorialBattle = TutorialBattle.Instance;
            _militaryManager = MilitaryManager.Instance;
        }

        public void BattleCase()
        {
            if (_tutorialData == null)
                _tutorialData = GetTutorialData();
            if (_tutorialData.StepTutorialCurrent == TutorialData.StepTutorial.TutorialStart)
            {
                TutorialTroopMove();
            }
            else if (_tutorialData.StepTutorialCurrent == TutorialData.StepTutorial.TutorialClickTown)
            {
                TutorialBattleBradit();
            }
        }

        //===================================================== battle step 1 =================================
     
        public void OnClickShowNextTepTutorial()
        {
            var step = _tutorialData.GetStepTutorial();
            switch (step)
            {
                case TutorialData.StepTutorial.TutorialCityBandit:
                    GetTutorialNpc().TutorialTalkBalloonContent.SetActive(false);
                    TutorialCityBandit();
                    break;
                case TutorialData.StepTutorial.TutorialTroopWin:
                    GetTutorialNpc().NpcContentStep1(true);
                    GetTutorialNpc().TutorialTalkBalloonContent.SetActive(true);
                    GetTutorialNpc().NpcTalkBalloon.ShowNextContent();
                    TutorialData.Instance.SetStepTutorial(TutorialData.StepTutorial.TutorialCityTooltipJoin);
                    break;
                case TutorialData.StepTutorial.TutorialCityTooltipJoin:
                    GetTutorialNpc().NpcContentStep1(false);
                    GetTutorialNpc().NpcTalkBalloon.ShowNextContent();
                    _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialTroopConfirmCity);
                    break;
                case TutorialData.StepTutorial.TutorialTroopConfirmCity:
                    GetTutorialNpc().NpcContentStep1(true);
                    _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialJoinTown);
                    GetTutorialNpc().NpcTalkBalloon.ShowNextContent();
                    TutorialTroopConfirmCity();
                    break;
                case TutorialData.StepTutorial.TutorialJoinTown:
                    TutorialJoinTown();
                    break;
                case TutorialData.StepTutorial.TutorialClickTown:
                    break;
                case TutorialData.StepTutorial.TutorialBattleBradit1:
                    StepTroopPlayerMoveGateTown();
                    break;
                case TutorialData.StepTutorial.TutorialBattleBradit2:
                    TutorialBattleBradit2();
                    break;
                case TutorialData.StepTutorial.TutorialBattleBradit3:
                    TutorialBattleBradit3();
                    break;
            }

        }

        public async void TutorialTroopMove()
        {
            GetTutorialNpc().DisableSet1(false);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialTroopMove));
            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialTroopMove);

            WorldMapTroopModel modelCavalry = GetTroopCavalryMeTutorial();
            WorldMapTroopModel modelArcher = GetTroopArcherMeTutorial();
            WorldMapTroopModel modelEnemy = GetTroopEnemyTutorial();

            if(modelEnemy == null || modelArcher == null || modelCavalry == null)
                return;

            var cityTroopStop = tableCityModels.GetByKey(95);
            modelCavalry.SetMatrixMove(modelCavalry.troopData);
            modelArcher.SetMatrixMove(modelArcher.troopData);
            modelCavalry.AnimationMove();
            modelArcher.AnimationMove();

            modelArcher.ChangeRotation(modelArcher.transform, cityTroopStop.Position, cityTroopStop.Position, 200);
            modelCavalry.ChangeRotation(modelCavalry.transform, cityTroopStop.Position, cityTroopStop.Position, 200);
            StartCoroutine(MoveToCameraZoomInSlow(modelCavalry, wm.zoomLevel, 0.2f, async () =>
            {
                posTarget = modelCavalry.transform.position;
                StartCoroutine(MoveToObject(cityTroopStop.Position, 0.002f));
                StartCoroutine(MoveToCameraSmooth(0.2f));
                modelArcher.MoveTo(modelArcher, cityTroopStop.Position, cityTroopStop.Position, 0.2f, () => { });
                modelCavalry.MoveTo(modelCavalry, cityTroopStop.Position, cityTroopStop.Position, 0.2f, async () =>
                {
                    modelArcher.troopData.Position = 95;
                    modelCavalry.troopData.Position = 95;
                    StopCoroutine("MoveToObject");
                    StopCoroutine("MoveToCameraSmooth");
                    var targetCalvary = new Vector3(modelCavalry.transform.position.x + 0.4f,
                        modelCavalry.transform.position.y, modelCavalry.transform.position.z);
                    var target = new Vector3(modelEnemy.transform.position.x + 0.4f, modelEnemy.transform.position.y,
                        modelEnemy.transform.position.z);
                    var pointBattle = Vector3.Lerp(target, targetCalvary, 0.5f);
                    StartCoroutine(MoveToObject(pointBattle, 0.006f));
                    StartCoroutine(MoveToCameraSmooth(0.2f));

                    await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeNpcTalkBalloon1));
                    modelArcher.AnimationIdle();
                    modelCavalry.AnimationIdle();
                });

                await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialMoveCameraX));
                // Show NPC start
                GetTutorialNpc().OpenUiPanelTutorialStart();
                await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeMoveCameraNpc1));
            
                _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialCityBandit);

                await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialJoinTown));
                // close NPC start
                GetTutorialNpc().CloseUiPanelTutorialStart();

                await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialJoinTown));
           
                // Show NPC continue
                GetTutorialNpc().OpenUiPanelTutorialNpc();
            }));

        }
        
        public void TutorialCityBandit()
        {
            GetTutorialNpc().NpcContentStep1(false);
            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialCityBandit);
            WorldMapTroopModel modelMe = GetTroopCavalryMeTutorial();
            WorldMapTroopModel modelEnemy = GetTroopEnemyTutorial();
            modelEnemy.SetMsgTalk(false, "Please help us !!!", WorldMapTroopModel.Talk.Kill);
            StopAllCoroutines();
            ShowNPC_1(modelMe, modelEnemy);
        }

        private async void ShowNPC_1(WorldMapTroopModel model, WorldMapTroopModel modelEnemy)
        {
            model.SetMsgTalk(true, "Kill them all !!!", WorldMapTroopModel.Talk.Kill);
            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialTroopTooltipKill);
            //Show kill them all
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialTroopMoveAttack));
            WorldMapTroopModel modelCMe = GetTroopCavalryMeTutorial();
            modelCMe.AnimationMove();

            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialTroopMoveAttack);
            GetTutorialNpc().LandContextMenu.OnAttackTutorial();
            wm.SetRotationCamera(new Vector3(45, 0, 0), 1.0f);
        }

        public async void TutorialTroopConfirmCity()
        {

            WorldMapTroopModel modelArcher = GetTroopArcherMeTutorial();
            WorldMapTroopModel modelCavalry = GetTroopCavalryMeTutorial();
            modelArcher.AnimationMove();
            modelCavalry.AnimationMove();
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialJoinTown));
          
            modelCavalry.TutorialJoinTown(0.015f);
            modelArcher.TutorialJoinTown(0.005f);
            GetTutorialNpc().NpcTalkBalloon.ShowNextContent();
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialJoinTown));
            OnClickShowNextTepTutorial();
        }

        public async void TutorialJoinTown()
        {
            GetTutorialNpc().DisableSet1(true);
            WorldMapTroopModel modelArcher = GetTroopArcherMeTutorial();
            WorldMapTroopModel modelCavalry = GetTroopCavalryMeTutorial();
           
            GetTutorialNpc().NpcTalkBalloon.ShowNextContent();
            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialClickTown);
            modelArcher.AnimationMove();
            modelCavalry.AnimationMove();
            modelCavalry.TutorialJoinTown(0.015f);
            modelArcher.TutorialJoinTown(0.005f);
            GetTutorialNpc().TutorialBlocker.SetActive(false);
            GetTutorialNpc().NpcContentStep1(false);
            GetTutorialNpc().TutorialTalkBalloonContent.SetActive(false);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialJoinTown));
            GetTutorialNpc().TutorialNPCPointer.ActivePointImage(true);
        }

        public void OnShowTutorialNpc(TutorialData.StepNpc step)
        {
            TutorialData.Instance.SetStepNpc(step);
            GetTutorialNpc().OpenNpcWar(step);
        }
        public void OnFinishShowText(object[] eventParam)
        {
            TutorialData.StepNpc step = TutorialData.Instance.GetStepNpc();
            GetTutorialNpc().CloseNpcWar((int)step);

            if(step == TutorialData.StepNpc.Npc8)
            {
                WorldMapTroopModel modelEnemy = GetTroopEnemyTutorial();
                var cityTroopEnemy = tableCityModels.GetByKey(modelEnemy.troopData.Position);
                GetTutorialNpc().OpenNpcClickCity(cityTroopEnemy.Position);
            }
        }

        public void OnClickShowNextTepTutorialEvt(object[] eventParam)
        {
            OnClickShowNextTepTutorial();
        }
        //=====================================================================================================

        //===================================================== battle step 7 =================================
        public async void TutorialBattleBradit()
        {
            GetTutorialNpc().DisableSet1(false);
            wm.SetRotationCamera(new Vector3(50, 0, 0), 1.0f);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialBattle1));
            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialBattleBradit1);

            WorldMapTroopModel modelEnemy = GetTroopEnemyTutorial();

            StartCoroutine(MoveToCameraZoomInSlow(modelEnemy, wm.zoomLevel, 0.18f, async () =>
            {
                WorldMapTroopModel modelAI1 = GetTroopAIMeTutorial(1);
                WorldMapTroopModel modelAI2 = GetTroopAIMeTutorial(2);
                WorldMapTroopModel modelEnemyAI1 = GetTroopEnemyAITutorial(1);
                WorldMapTroopModel modelEnemyAI2 = GetTroopEnemyAITutorial(2);
                WorldMapTroopModel modelCavalry = GetTroopCavalryMeTutorial();
                
                modelCavalry.ChangeRotation(modelCavalry.transform, modelEnemy.Position, modelEnemy.Position, 200);

                var cityTroopAi1 = new Vector3(1017.66f, 0, 762.147f);
                modelAI1.Position = cityTroopAi1;
                var cityTroopAi2 = new Vector3(1018.54f, 0, 760.722f);
                modelAI2.Position = cityTroopAi2;

                var cityTroopEnemyAi1 = new Vector3(1018.54f, 0, 768.04f);
                modelEnemyAI1.Position = cityTroopEnemyAi1;
                var cityTroopEnemyAi2 = new Vector3(1017.93f, 0, 768.885f);
                modelEnemyAI2.Position = cityTroopEnemyAi2;

                // show NPC
                OnShowTutorialNpc(TutorialData.StepNpc.Npc1);

                await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialBattleNpc1));
                OnFinishShowText(new object[0]);
                TutorialData.Instance.SetStepTutorial(TutorialData.StepTutorial.TutorialBattleBradit1);
                OnClickShowNextTepTutorial();

            }));
        }


        private async void StepTroopPlayerMoveGateTown()
        {
            WorldMapTroopModel moderEnemy = GetTroopEnemyTutorial();
            WorldMapTroopModel modelCavalry = GetTroopCavalryMeTutorial();
            
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeStepTroopPlayerMoveGateTown));
            StartCoroutine(MoveToCamera(modelCavalry, 0.1f, 0, 0));
            OnShowTutorialNpc(TutorialData.StepNpc.Npc2);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowNpc2));
            OnFinishShowText(new object[1]);
            
            modelCavalry.SetMatrixAttack();
            modelCavalry.AnimationMove();
            var pointCavalryGate = Vector3.Lerp(modelCavalry.transform.position, moderEnemy.transform.position, 0.25f);
            var pointArcherGate = Vector3.Lerp(modelCavalry.transform.position, moderEnemy.transform.position, 0.15f);
            modelCavalry.ChangeRotationSmooth(modelCavalry.transform, pointCavalryGate, pointCavalryGate, 200);
            modelCavalry.MoveTo(modelCavalry, pointCavalryGate, pointCavalryGate, 0.1f, () => { });

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeDelayMoveArcherToBattle));
            
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowNpc3));
            OnShowTutorialNpc(TutorialData.StepNpc.Npc3);


            TutorialData.Instance.SetStepTutorial(TutorialData.StepTutorial.TutorialBattleBradit2);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialBattle2));
            modelCavalry.SetMsgTalk(true, "", WorldMapTroopModel.Talk.Ready);
            StartCoroutine(MoveToCamera(modelCavalry, 0.1f, 0, 0));
            wm.SetRotationCamera(new Vector3(45, 0, 0), 1.0f);
            MoveBattleCenter();
            OnClickShowNextTepTutorial();

        }

        private async void TutorialBattleBradit2()
        {
            WorldMapTroopModel modelCavalry = GetTroopCavalryMeTutorial();
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowNpc4));

            OnShowTutorialNpc(TutorialData.StepNpc.Npc4);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowNpc5));
            OnShowTutorialNpc(TutorialData.StepNpc.Npc5);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowNpc6));
            OnShowTutorialNpc(TutorialData.StepNpc.Npc6);
            
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialBattleNpc3));
            TutorialData.Instance.SetStepTutorial(TutorialData.StepTutorial.TutorialBattleBradit3);
            OnClickShowNextTepTutorial();
        }

        private async void TutorialBattleBradit3()
        {
            WorldMapTroopModel moderEnemy = GetTroopEnemyTutorial();
            WorldMapTroopModel modelAi2 = GetTroopAIMeTutorial(1);
            WorldMapTroopModel modelAi3 = GetTroopAIMeTutorial(2);
            WorldMapTroopModel modelBradit2 = GetTroopEnemyAITutorial(1);
            WorldMapTroopModel modelBradit3 = GetTroopEnemyAITutorial(2);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowTooltipTalk));
            modelAi2.SetMsgTalk(true, "", WorldMapTroopModel.Talk.Freedom);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowTooltipTalk));
            modelAi3.SetMsgTalk(true, "", WorldMapTroopModel.Talk.Fight);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowTooltipTalk));
            modelBradit2.SetMsgTalk(true, "", WorldMapTroopModel.Talk.What);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowTooltipTalkFallBack));
            modelBradit3.SetMsgTalk(true, "", WorldMapTroopModel.Talk.Fallback);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowTooltipTalk));
            OnShowTutorialNpc(TutorialData.StepNpc.Npc7);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowNpc8));
            OnShowTutorialNpc(TutorialData.StepNpc.Npc8);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeTutorialCity));
            _tutorialData.SetStepTutorial(TutorialData.StepTutorial.TutorialBatleBraditCity);
            //var cityTroopEnemy = tableCityModels.GetByKey(moderEnemy.troopData.Position);
            //GetTutorialNpc().OpenNpcClickCity(cityTroopEnemy.Position);
        }
        //=====================================================================================================

        //===================================================== battle move step 7 ============================
        public async void MoveBattleCenter()
        {
            WorldMapTroopModel moderEnemy = GetTroopEnemyTutorial();
            WorldMapTroopModel moderCavalryMe = GetTroopCavalryMeTutorial();
            moderEnemy.AnimationMove();
            moderCavalryMe.AnimationMove();
            moderEnemy.SetMatrixAttack(moderEnemy.troopData, false);
            moderCavalryMe.SetMatrixAttack(moderCavalryMe.troopData, false);

            var pointBattle1 = Vector3.Lerp(moderCavalryMe.transform.position, moderEnemy.transform.position, 0.25f);

            var pointBattleMe = Vector3.Lerp(moderCavalryMe.transform.position, moderEnemy.transform.position, 0.28f);
            var pointBattleEnemy = Vector3.Lerp(moderEnemy.transform.position, moderCavalryMe.transform.position, 0.68f);

            moderEnemy.ChangeRotationSmooth(moderEnemy.transform, pointBattleEnemy, pointBattleEnemy, 200);
            moderEnemy.MoveTo(moderEnemy, pointBattleEnemy, pointBattleEnemy, 0.25f, async () =>
            {
                var targetPos = new Vector3(pointBattleEnemy.x, pointBattleEnemy.y, pointBattleEnemy.z - 0.1f);
                moderEnemy.SetMatrixMoveMooth();
                moderEnemy.ChangeRotationSmooth(moderEnemy.transform, targetPos, targetPos, 2000);
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                moderEnemy.SetMatrixAttack();
                moderEnemy.AnimationAttack();
                moderCavalryMe.AnimationAttack();

            });

            moderCavalryMe.ChangeRotationSmooth(moderCavalryMe.transform, pointBattleMe, pointBattleMe, 200);
            moderCavalryMe.MoveTo(moderCavalryMe, pointBattleMe, pointBattleMe, 0.1f, async () =>
            {
                var targetPos = new Vector3(pointBattleMe.x, pointBattleMe.y, pointBattleMe.z + 0.1f);
                moderCavalryMe.SetMatrixMoveMooth();
                moderCavalryMe.ChangeRotationSmooth(moderCavalryMe.transform, targetPos, targetPos, 2000);
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                moderCavalryMe.SetMatrixAttack();
              
            });
            
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeMoveBattleCenter));
   


            moderEnemy.troopData.AttackTroop = moderCavalryMe.troopData.IdTroop;
            moderEnemy.troopData.AttackLand = 0;
            moderEnemy.troopData.StatusBattle = (int)StatesGlobal.STATUS_BATTLE.EMPTY;
            moderCavalryMe.troopData.AttackTroop = moderEnemy.troopData.IdTroop;
            moderCavalryMe.troopData.AttackLand = moderEnemy.troopData.AttackLand;
            moderCavalryMe.troopData.StatusBattle = (int)StatesGlobal.STATUS_BATTLE.EMPTY;
            
            troopService.SendTroopMove(moderCavalryMe.troopData, moderEnemy.troopData.Position);
            _militaryManager.OnStartBattleTutorial();

            var pointBattle2 = new Vector3(pointBattle1.x + 0.3f, pointBattle1.y, pointBattle1.z);
            var pointBattle3 = new Vector3(pointBattleMe.x - 0.23f, pointBattleMe.y, pointBattleMe.z);
            
            MoveBattleEnemy(pointBattle2, pointBattle3);
            TutorialBattle.Instance.StepBattle(TutorialBattle.StepBattleTutorial.Step1);
            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeDelayMovePlayerAi));
 
            MoveBattlePlayerAi(pointBattle2, pointBattle3);
        }


        public async void MoveBattleEnemy(Vector3 pointBattle2, Vector3 pointBattle3)
        {
            WorldMapTroopModel moderAi2 = GetTroopAIMeTutorial(1);
            WorldMapTroopModel moderAi3 = GetTroopAIMeTutorial(2);
            WorldMapTroopModel moderEnemyAi1 = GetTroopEnemyAITutorial(1);
            WorldMapTroopModel moderEnemyAi2 = GetTroopEnemyAITutorial(2);
            moderEnemyAi1.AnimationMove();
            moderEnemyAi2.AnimationMove();
            moderEnemyAi1.SetMatrixAttack(moderEnemyAi1.troopData, false);
            moderEnemyAi2.SetMatrixAttack(moderEnemyAi2.troopData, false);

            pointBattle2 = new Vector3(pointBattle2.x, pointBattle2.y, pointBattle2.z + 0.04f);
            moderEnemyAi1.ChangeRotationSmooth(moderEnemyAi1.transform, moderAi2.Position, moderAi2.Position, 200);
            moderEnemyAi1.MoveTo(moderEnemyAi1, pointBattle2, pointBattle2, 0.11f, () =>
            {
                moderEnemyAi1.AnimationAttack();
            });

            pointBattle3 = new Vector3(pointBattle3.x, pointBattle3.y, pointBattle3.z + 0.04f);
            moderEnemyAi2.ChangeRotationSmooth(moderEnemyAi2.transform, moderAi3.Position, moderAi3.Position, 200);
            moderEnemyAi2.MoveTo(moderEnemyAi2, pointBattle3, pointBattle3, 0.13f, () =>
            {
                moderEnemyAi2.AnimationAttack();
            });

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeDelayStartBattleStep2));
            TutorialBattle.Instance.StepBattle(TutorialBattle.StepBattleTutorial.Step2);
        }

        public async void MoveBattlePlayerAi(Vector3 pointBattle2, Vector3 pointBattle3)
        {
            WorldMapTroopModel moderEnemyAi2 = GetTroopEnemyAITutorial(1);
            WorldMapTroopModel moderEnemyAi3 = GetTroopEnemyAITutorial(2);
            WorldMapTroopModel moderTroopAi1 = GetTroopAIMeTutorial(1);
            WorldMapTroopModel moderTroopAi2 = GetTroopAIMeTutorial(2);
            moderTroopAi1.AnimationMove();
            moderTroopAi2.AnimationMove();
            moderTroopAi1.SetMatrixAttack(moderTroopAi1.troopData, false);
            moderTroopAi2.SetMatrixAttack(moderTroopAi2.troopData, false);
            
            moderTroopAi1.ChangeRotationSmooth(moderTroopAi1.transform, pointBattle2, pointBattle2, 200);
            moderTroopAi1.MoveTo(moderTroopAi1, pointBattle2, pointBattle2, 0.085f, () =>
            {
                moderTroopAi1.AnimationAttack();
            });

            moderTroopAi2.ChangeRotationSmooth(moderTroopAi2.transform, pointBattle3, pointBattle3, 200);
            moderTroopAi2.MoveTo(moderTroopAi2, pointBattle3, pointBattle3, 0.145f, () =>
            {
                moderTroopAi2.AnimationAttack();
            });

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeDelayChangeRotationTroopAi));
            moderTroopAi1.ChangeRotationSmooth(moderTroopAi1.transform, moderEnemyAi2.Position, moderEnemyAi2.Position, 200);
            moderTroopAi2.ChangeRotationSmooth(moderTroopAi2.transform, moderEnemyAi3.Position, moderEnemyAi3.Position, 200);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeShowBattleStep3));
            TutorialBattle.Instance.StepBattle(TutorialBattle.StepBattleTutorial.Step3);

            await Task.Delay(TimeSpan.FromMilliseconds(_tutorialData.TimeDelayDoneTutorial));
       
            var modelEnemy = GetTroopEnemyTutorial();
            modelEnemy.Lose();
            moderEnemyAi2.Lose();
            moderEnemyAi3.Lose();
            modelEnemy.OnDisableParticeBattle();
            moderEnemyAi2.OnDisableParticeBattle();
            moderEnemyAi3.OnDisableParticeBattle();

            var modelMe = GetTroopCavalryMeTutorial();
            modelMe.AnimationFinish();
            moderTroopAi1.AnimationFinish();
            moderTroopAi2.AnimationFinish();
            modelMe.OnDisableParticeBattle();
            moderTroopAi1.OnDisableParticeBattle();
            moderTroopAi2.OnDisableParticeBattle();
            
            TutorialBattle.Instance.StepBattle(TutorialBattle.StepBattleTutorial.Done);
        
        }

        //=====================================================================================================

        //===================================================== battle utils ==================================
        public WorldMapTroopModel GetTroopCavalryMeTutorial()
        {
            var troopModels = MilitaryManager.Instance.troopModels;
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer == userProfile.id)
                {
                    if (model.troopData.Value_Cavalry > 0 && model.troopData.Buff_1 == 0 && model.troopData.Buff_2 == 0)
                        return model;

                }

            }
            return null;
        }

        public WorldMapTroopModel GetTroopArcherMeTutorial()
        {
            var troopModels = MilitaryManager.Instance.troopModels;
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer == userProfile.id)
                {
                    if (model.troopData.Value_Archer > 0 && model.troopData.Buff_1 == 0 && model.troopData.Buff_2 == 0)
                    {
                        if (_tutorialData.GetStepTutorial() > TutorialData.StepTutorial.TutorialClickTown)
                            model.gameObject.SetActive(false);
                        return model;
                    }

                }

            }
            return null;
        }

        public WorldMapTroopModel GetTroopAIMeTutorial(int buff)
        {
            var troopModels = MilitaryManager.Instance.troopModels;
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer == userProfile.id)
                {
                    if (buff == 1)
                    {
                        if (model.troopData.Buff_1 == 1)
                            return model;
                    }

                    if (buff == 2)
                    {
                        if (model.troopData.Buff_2 == 1)
                            return model;
                    }

                }

            }
            return null;
        }

        public WorldMapTroopModel GetTroopEnemyTutorial()
        {
            var troopModels = MilitaryManager.Instance.troopModels;
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer != userProfile.id)
                {
                    return model;
                }

            }
            return null;
        }

        public WorldMapTroopModel GetTroopEnemyAITutorial(int buff)
        {
            var troopModels = MilitaryManager.Instance.troopModels;
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer != userProfile.id)
                {
                    if (buff == 1)
                    {
                        if (model.troopData.Buff_1 == 1)
                            return model;
                    }

                    if (buff == 2)
                    {
                        if (model.troopData.Buff_2 == 1)
                            return model;
                    }
                }

            }
            return null;
        }

        private void OnZoomCamera(object[] eventParam)
        {
            WorldMapTroopModel model = (WorldMapTroopModel)eventParam[0];
            float zoom = (float)eventParam[1];
            bool isZoomOut = (bool)eventParam[2];
            if (isZoomOut)
            {
                StopAllCoroutines();
                StartCoroutine(MoveToCameraZoomOutSlow(model, wm.zoomLevel, zoom, () => { }));
            }
            else
            {
                StartCoroutine(MoveToCameraZoomInSlow(model, wm.zoomLevel, zoom, () => { }));
            }
        }

        private void OnMoveCamera(object[] eventParam)
        {
            WorldMapTroopModel modeltarget = (WorldMapTroopModel)eventParam[0];
            float zoom = (float)eventParam[1];
            StartCoroutine(MoveToCamera(modeltarget, zoom, 0, 0));
        }

        public IEnumerator MoveToCamera(WorldMapTroopModel modeltarget, float zoom,float posRaiseX,float posRaiseZ)
        {
            yield return new WaitForFixedUpdate();
            Vector3 target = modeltarget.Position;
            if (posRaiseX != 0)
                target = new Vector3(target.x + posRaiseX, target.y, target.z);
            if (posRaiseZ != 0)
                target = new Vector3(target.x, target.y, target.z + posRaiseZ);
            wm.MoveTo(target, zoom);
            StartCoroutine(MoveToCamera(modeltarget, zoom, posRaiseX, posRaiseZ));
        }

        public IEnumerator MoveToCameraSmooth(float zoom)
        {
            yield return new WaitForFixedUpdate();
            wm.MoveTo(posTarget, zoom);
            StartCoroutine(MoveToCameraSmooth(zoom));
        }

        public IEnumerator MoveToCameraX(WorldMapTroopModel modeltarget, float zoom)
        {
            yield return new WaitForFixedUpdate();
            wm.MoveToX(modeltarget, zoom);
            StartCoroutine(MoveToCameraX(modeltarget, zoom));
        }

        public IEnumerator MoveToCameraZ(WorldMapTroopModel modeltarget, float zoom)
        {
            yield return new WaitForFixedUpdate();
            wm.MoveToZ(modeltarget, zoom);
            StartCoroutine(MoveToCameraZ(modeltarget, zoom));
        }

        private Vector3 posTarget;
        public IEnumerator MoveToObject(Vector3 target, float speed)
        {
            Vector3 b = target;
            float step = (speed / (posTarget - b).magnitude) * Time.fixedDeltaTime;
            float t = 0;
            while (t <= 1.0f)
            {
                t += step; // Goes from 0 to 1, incrementing by step each time
                posTarget = Vector3.Lerp(posTarget, b, t); // Move objectToMove closer to b
                yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
            }
            posTarget = b;
        }

        public IEnumerator MoveToCameraZoomInSlow(WorldMapTroopModel modeltarget, float zoom, float minZoom, Action onComplete)
        {
            yield return new WaitForFixedUpdate();
            if (zoom >= minZoom)
            {
                zoom = zoom - 0.002f;

                Vector3 pointZoom = new Vector3(modeltarget.Position.x - 0.1f, modeltarget.Position.y,
                    modeltarget.Position.z - 0.1f);
                wm.MoveTo(pointZoom, zoom);
                StartCoroutine(MoveToCameraZoomInSlow(modeltarget, zoom, minZoom, onComplete));
            }
            else
            {
                StopCoroutine("MoveToCameraZoomInSlow");
                onComplete?.Invoke();
            }
        }

        public IEnumerator MoveToCameraZoomOutSlow(WorldMapTroopModel modeltarget, float zoom, float minZoom, Action onComplete)
        {
            yield return new WaitForFixedUpdate();
            if (zoom < minZoom)
            {
                zoom = zoom + 0.002f;

                Vector3 pointZoom = new Vector3(modeltarget.Position.x - 0.1f, modeltarget.Position.y,
                    modeltarget.Position.z - 0.1f);
                wm.MoveTo(pointZoom, zoom);
                StartCoroutine(MoveToCameraZoomOutSlow(modeltarget, zoom, minZoom, onComplete));
            }
            else
            {
                StopAllCoroutines();
                onComplete?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            Destroy(this);
        }
        //=====================================================================================================
    }
}

