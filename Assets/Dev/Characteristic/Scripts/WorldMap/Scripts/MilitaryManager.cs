using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Assets.Dev.Tutorial.Scripts;
using Fbs;
using UnityEditor;
using UnityEngine;
using WorldMapStrategyKit;
using Vector3 = UnityEngine.Vector3;

namespace CoreData.UniFlow.Common
{
    public class MilitaryManager : MonoSingleton<MilitaryManager>
    {
        [SerializeField] private EndBattlePopup UIEndBattleVictory;

        [SerializeField] private EndBattlePopup UIEndBattleTerritory;

        [SerializeField] private Vector3[] positionTroops;
        
        [SerializeField]
        private GameEvent onAttackTutorial;

        [SerializeField]
        private GameEvent onMoveCamera;
        
        [SerializeField]
        private GameEvent onZoomCamera;
        
        //WMSK map;
        public WorldMapControllerV3 worldMap;

        public WorldMapTroopService troopService;
        public TableWorldMapPlayer tableWorldPlayers;
        public TableCityViewModel tableCityModels;
        public LandStaticInfoList landList;
        public LandAndKingdomService landAndKingdomService;
        public LandController landController;
        public TutorialWorldmap TutorialWorldmap;
        public TutorialNpc TutorialNpc;

        public IntegerVariable selectedCity;

        public UserProfile userProfile;
        public CityList cities;
        public GameEvent notification;

        public PoolService pool;

        public Dictionary<long, WorldMapTroopModel> troopModels = new();
        
        public void OnEnable()
        {
            if (StatesGlobal.UID_PLAYER > 0)
            {
                _ = APIManager.Instance.RequestJoinWorldMap();
            }
            //map = WMSK.instance;
            //InitData();
            TutorialWorldmap = TutorialWorldmap.Instance;
            tableWorldPlayers.OnSetData += OnPlayer;
            troopService.troopRepo.OnSetData += OnTroopValueChanged;
            landAndKingdomService.OnLandUpdate += OnLandUpdate;
            onAttackTutorial.Subcribe(OnAttackTutorial);
        }

        private void OnDisable()
        {
            onAttackTutorial.Unsubcribe(OnAttackTutorial);
            tableWorldPlayers.OnSetData -= OnPlayer;
            troopService.troopRepo.OnSetData -= OnTroopValueChanged;
            landAndKingdomService.OnLandUpdate -= OnLandUpdate;
        }

        void OnLandUpdate(LandDynamicInfo land, bool change)
        {
            if (change)
            {
                notification.Raise($"Land {land.id} have been occipy by kingdom {land.idParent}!!");
            }
        }

        void OnTroopValueChanged(WorldMapTroop troop)
        {
            if (troopService.exists(troop))
            {
                UpdateTroopView(troop);
            }
            else if (troopModels.ContainsKey(troop.IdTroop))
            {
                var troopModel = troopModels[troop.IdTroop];
                pool.Release(0, troopModel.transform);
                troopModel.gameObject.SetActive(false);
                troopModels.Remove(troop.IdTroop);
                notification.Raise($"Troop of player {troop.idPlayer} is defeated!!");
            }
        }

        void OnPlayer(WorldMapPlayer player)
        {
        }

        void UpdateTroopView(WorldMapTroop troop)
        {
            CityViewModel city = null;
            WorldMapPlayer player = tableWorldPlayers.GetByKey(troop.idPlayer);
            if (troop.Position > 0)
            {
                city = tableCityModels.GetByKey(troop.Position);
            }
            else //if (troop.idPlayer == userProfile.id)
            {
                var p = tableWorldPlayers.GetByKey(troop.idPlayer);
                //var userCities = cities.FindBy_idLand(p.IdLand);
                var land = landList.GetLand(p.IdLand);
                if (land.cities.Count > 0)
                {
                    city = tableCityModels.GetByKey(land.cities[0]);
                    troop.Position = city.Uid;
                }
            }

            CityViewModel targetCity = null;
            if (troop.TargetPosition > 0)
            {
                targetCity = tableCityModels.GetByKey(troop.TargetPosition);
            }

            if (city == null) return;

            var cityPos = worldMap.transform.TransformPoint(city.Position);
            var cityPosTarget = cityPos;
            if (targetCity != null)
                cityPosTarget = worldMap.transform.TransformPoint(targetCity.Position);


            WorldMapTroopModel model;
            int addRandom = UnityEngine.Random.Range(0, positionTroops.Length - 1);
            if (troopModels.TryGetValue(troop.IdTroop, out model))
            {
                model.gameObject.SetActive(true);
                //if (model.troopData.StatusBattle != (int)StatesGlobal.STATUS_BATTLE.BATTLE)
                //{
                //    model.setData(troop);
                //    model.Position = cityPos;
                //}
            }
            else
            {
                var t = pool.Get(0, transform);
                t.gameObject.name = $"{troop.idPlayer} - {troop.IdTroop}";
                t.transform.position = Vector3.zero;
                model = t.GetComponent<WorldMapTroopModel>();
                if (model != null)
                {
                    model.setData(troop);
                    model.transform.localScale = Vector3.one / 5.0f;
                    model.gameObject.SetActive(true);
                    if (model.troopData.StatusBattle != (int)StatesGlobal.STATUS_BATTLE.BATTLE)
                        model.Position = cityPos;


                    model.UpdateAnim("IsIdle", true);
                    if (model.troopData.idPlayer != userProfile.id)
                        model.SetMatrixIdle();

                    troopModels.Add(troop.IdTroop, model);
                }
            
            }
        }

        private void Update()
        {
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);

            if (TutorialWorldmap.GetTutorialData().TutorialTracker.IsNeedTutorial())
            {
                var model = TutorialWorldmap.GetTroopCavalryMeTutorial();
                var modelEnemy = TutorialWorldmap.GetTroopEnemyTutorial();
                if (model == null || modelEnemy == null)
                    return;

                if (model.troopData.idPlayer == userProfile.id && Utils.IsCheckDistance(model.Position,
                                                                   modelEnemy.Position, 0.1f)
                                                               && model.troopData.idPlayer !=
                                                               modelEnemy.troopData.idPlayer)
                {
                    var kingdom = landAndKingdomService.motherLand(userProfile.landId);
                    var cityAttack = cities.GetCity(model.troopData.Position);
                    var attackKingdom = landAndKingdomService.motherLand(cityAttack.landId);

                    var troops = troopService.findEnemiesAtCity(kingdom, userProfile.id, model.troopData.Position);
                    if (troops.Count > 0)
                    {
                        var randomTroop = troops[UnityEngine.Random.Range(0, troops.Count)];
                        model.troopData.AttackTroop = randomTroop.IdTroop;
                        model.troopData.AttackLand = 0;
                        if (model.troopData.StatusBattle != (int)StatesGlobal.STATUS_BATTLE.BATTLE)
                            OnStartBattleTutorial();
                    }
                    else if (kingdom != attackKingdom ||
                             (attackKingdom == 0 &&
                              cityAttack.landId != userProfile.landId) &&
                             landAndKingdomService.isLandCapital(model.troopData.Position)
                            )
                    {
                        model.troopData.AttackLand = cityAttack.landId;
                        model.troopData.AttackTroop = 0;
                    }

                }
            }
            else
            {
                for (int i = 0; i < models.Length; i++)
                {
                    var model = models[i];
                    if (model.troopData.idPlayer == userProfile.id
                        && landAndKingdomService.isLandCapital(model.troopData.Position)
                       )
                    {
                        var kingdom = landAndKingdomService.motherLand(userProfile.landId);
                        var cityAttack = cities.GetCity(model.troopData.Position);
                        var attackKingdom = landAndKingdomService.motherLand(cityAttack.landId);

                        var troops = troopService.findEnemiesAtCity(kingdom, userProfile.id, model.troopData.Position);
                        if (troops.Count > 0)
                        {
                            var randomTroop = troops[UnityEngine.Random.Range(0, troops.Count)];
                            model.troopData.AttackTroop = randomTroop.IdTroop;
                            model.troopData.AttackLand = 0;
                            if (model.troopData.StatusBattle != (int)StatesGlobal.STATUS_BATTLE.BATTLE)
                                OnStartBattle();
                        }
                        else if (kingdom != attackKingdom ||
                                 (attackKingdom == 0 &&
                                  cityAttack.landId != userProfile.landId) &&
                                 landAndKingdomService.isLandCapital(model.troopData.Position)
                                )
                        {
                            model.troopData.AttackLand = cityAttack.landId;
                            model.troopData.AttackTroop = 0;
                        }

                    }
                }
            }

        }
        

        public void OnMoveGateCapitalTutorial()
        {
            WorldMapTroopModel moderEnemy = TutorialWorldmap.GetTroopEnemyTutorial();
            WorldMapTroopModel moderCavalryMe = TutorialWorldmap.GetTroopCavalryMeTutorial();
            moderEnemy.AnimationMove();
            moderCavalryMe.AnimationMove();
            var targetCity = tableCityModels.GetByKey(moderEnemy.troopData.Position);
            
            float randomPos = UnityEngine.Random.Range(0.3f, 0.7f);
            var targetGat = new Vector3(targetCity.Position.x + randomPos, targetCity.Position.y,
                targetCity.Position.z);

            moderEnemy.ChangeRotation(moderEnemy.transform, moderCavalryMe.Position, moderCavalryMe.Position, 200);
            moderEnemy.MoveTo(moderEnemy, targetGat, targetGat, 0.1f, () => { });

        }

        public void MatrixStartBattle()
        {
            WorldMapTroopModel moderEnemy = TutorialWorldmap.GetTroopEnemyTutorial();
            WorldMapTroopModel moderArcherMe = TutorialWorldmap.GetTroopArcherMeTutorial();
            WorldMapTroopModel moderCavalryMe = TutorialWorldmap.GetTroopCavalryMeTutorial();
            moderEnemy.AnimationMove();
            moderCavalryMe.AnimationMove();
   
            var pointBattle = Vector3.Lerp(moderCavalryMe.transform.position, moderEnemy.transform.position, 0.5f);
            var pointBattleArcher = Vector3.Lerp(moderCavalryMe.transform.position, moderEnemy.transform.position, 0.4f);

            moderArcherMe.ChangeRotation(moderArcherMe.transform, pointBattleArcher, pointBattleArcher, 200);
            moderEnemy.MoveTo(moderEnemy, pointBattle, pointBattle, 0.1f, () =>
            {
                moderEnemy.AnimationAttack();
            });

            moderCavalryMe.ChangeRotationSmooth(moderCavalryMe.transform, pointBattle, pointBattle, 200);
            moderCavalryMe.MoveTo(moderCavalryMe, pointBattle, pointBattle, 0.3f, () =>
            {
                moderCavalryMe.AnimationAttack();
                moderArcherMe.AnimationAttack();
            });

            troopService.SendTroopMove(moderArcherMe.troopData, moderEnemy.troopData.Position);
            troopService.SendTroopMove(moderCavalryMe.troopData,moderEnemy.troopData.Position);
        }

        public void OnStartBattleTutorial()
        {
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            WorldMapTroopModel moderArcherMe = TutorialWorldmap.GetTroopArcherMeTutorial();
            WorldMapTroopModel moderCavalryMe = TutorialWorldmap.GetTroopCavalryMeTutorial();
            WorldMapTroopModel moderEnemy = TutorialWorldmap.GetTroopEnemyTutorial();
            if (moderEnemy.troopData.AttackTroop > 0 || moderCavalryMe.troopData.AttackTroop > 0)
            {
                if (moderEnemy.AttackCooldown <= 0 || moderCavalryMe.AttackCooldown <= 0)
                {
                    if (moderCavalryMe.troopData.StatusBattle != (int)StatesGlobal.STATUS_BATTLE.BATTLE ||
                        moderEnemy.troopData.StatusBattle != (int)StatesGlobal.STATUS_BATTLE.BATTLE)
                    {
                        moderCavalryMe.attackTarget = moderCavalryMe.transform.position;
                        moderEnemy.attackTarget = moderCavalryMe.transform.position;
                        moderCavalryMe.AttackCooldown = 1.0f;
                        moderEnemy.AttackCooldown = 1.0f;
                        int idTroopModel = moderCavalryMe.troopData.IdTroop;
                        int idTroopEnemyModel = moderEnemy.troopData.IdTroop;
                        moderEnemy.troopData.AttackTroop = idTroopModel;
                        moderCavalryMe.troopData.AttackTroop = idTroopEnemyModel;
                    }

                    onZoomCamera?.Raise(moderCavalryMe, 0.1f, false);

                    moderCavalryMe.troopData.StatusBattle = (int)StatesGlobal.STATUS_BATTLE.BATTLE;
                    moderEnemy.troopData.StatusBattle = (int)StatesGlobal.STATUS_BATTLE.BATTLE;
                    _ = troopService.Attack(moderCavalryMe.troopData, moderEnemy.troopData, 0);

                    //float timeDelay = 10;
                    //if (TutorialData.Instance.GetStepTutorial() >= TutorialData.StepTutorial.TutorialBattleBradit1)
                    //    timeDelay = 30;
                    //EndBattleIfNotNetwork(timeDelay);
                }
            }
        }

        public void OnAttackTutorial(object[] eventParam)
        {
            var troops = troopService.findByPlayerAndType(userProfile.id, 0);
           
            List<WorldMapTroop> selectedTroops = new List<WorldMapTroop>();
            foreach (var view in troops)
            {
                if (view.IdTroop > 0)
                {
                    selectedTroops.Add(view);
                }
            }

            var error = troopService.MoveToCity(selectedTroops, selectedCity.Value);
            if (error.Count > 0)
            {
                foreach (var t in error)
                {
                    Debug.Log($"Can move {t.IdTroop}");
                }
            }
        }
        
        //============================================================
      
        public void OnStartBattle()
        {
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);

            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.AttackTroop > 0 || model.troopData.AttackLand > 0)
                {
                    if (model.AttackCooldown <= 0)
                    {
                        if (model.troopData.AttackTroop > 0)
                        {
                            var enemy = troopService.Get(model.troopData.AttackTroop);
                            if (troopService.exists(enemy))
                            {
                                var enemyModel = troopModels[model.troopData.AttackTroop];

                                if (model.troopData.idPlayer == userProfile.id)
                                {
                                    model.troopData.StatusBattle = (int)StatesGlobal.STATUS_BATTLE.BATTLE;
                                    _ = troopService.Attack(model.troopData, enemy, 0);
                                }

                                model.attackTarget = enemyModel.transform.position;
                                model.AttackCooldown = 1.0f;
                            }
                            else
                            {
                                model.troopData.AttackTroop = 0;
                            }
                        }
                        else if (model.troopData.AttackLand > 0)
                        {

                            var attackLand = landList.GetLand(model.troopData.AttackLand);
                            var capital = tableCityModels.GetByKey(attackLand.cities[0]);

                            var kingdom = landAndKingdomService.motherLand(userProfile.landId);
                            var attackKingdom = landAndKingdomService.motherLand(model.troopData.AttackLand);


                            model.attackTarget = worldMap.transform.TransformPoint(capital.Position);
                            ;
                            if (model.troopData.idPlayer == userProfile.id)
                            {
                                if (kingdom != attackKingdom)
                                {
                                    model.troopData.StatusBattle = (int)StatesGlobal.STATUS_BATTLE.BATTLE;
                                    _ = troopService.Attack(model.troopData, null, model.troopData.AttackLand);
                                    model.AttackCooldown = 1.0f;
                                }
                                else
                                {
                                    model.troopData.AttackLand = 0;
                                }
                            }
                        }
                    }

                }
                else if (model.troopData.Position != model.troopData.TargetPosition)
                {
                    if (model.troopData.TargetPosition == 0)
                        model.troopData.TargetPosition = model.troopData.Position;

                    if (model.IsReachTargetCity)
                    {
                        _ = troopService.Reach(model.troopData);
                    }
                }
            }
        }

        public async void InitData()
        {
            await UniTask.DelayFrame(2);

            var troops = troopService.findAllExists();
            for (int i = 0; i < troops.Count; i++)
            {
                var troop = troops[i];
                UpdateTroopView(troop);
            }
        }
        

        private TypeBattle GetTypeBattle(MilitaryObject player)
        {
            List<PlayerInWorldMap> listPlayer = MasterDataStoreGlobal.Instance.PlayerInWorldMaps;
            for (int i = 0; i < listPlayer.Count; i++)
            {
                Province province = WMSK.instance.GetProvince(listPlayer[i].IdLand);
                List<City> citys = WMSK.instance.GetCities(province);
                if (citys != null)
                {
                    if (citys.Count > 0)
                    {
                        if (player.CityId == citys[0].uniqueId)
                            return TypeBattle.Territory;
                    }
                }
            }

            return TypeBattle.Victory;
        }

        public void PlayerLeaveWorldMap(long uid)
        {
            for (int i = 0; i < MasterDataStoreGlobal.Instance.PlayerInWorldMaps.Count; i++)
            {
                if (MasterDataStoreGlobal.Instance.PlayerInWorldMaps[i].Uid == uid)
                {
                    MasterDataStoreGlobal.Instance.PlayerInWorldMaps.RemoveAt(i);
                }
            }
        }

        public void UpdateBattle(UpdateBattle dataUpdateBattle)
        {
            if (TutorialData.Instance.GetStepTutorial() >= TutorialData.StepTutorial.TutorialBattleBradit1 &&
                TutorialData.Instance.GetStepTutorial() < TutorialData.StepTutorial.TutorialDone)
                return;

            int lengh = dataUpdateBattle.DataUpdateTroopInfoInWarLength;
            List<UpdateTroopInfoInWarData> troopList = new List<UpdateTroopInfoInWarData>();
            for (int i = 0; i < lengh; i++)
            {
                var troop = dataUpdateBattle.DataUpdateTroopInfoInWar(i);
                UpdateTroopInfoInWarData updateTroopInfoInWar = new UpdateTroopInfoInWarData();
                updateTroopInfoInWar.IdTroops = troop.Value.IdTroops;
                updateTroopInfoInWar.Damge = troop.Value.Damge;
                updateTroopInfoInWar.TotalPaws = troop.Value.TotalPaws;
                updateTroopInfoInWar.TotalPawsRemain = troop.Value.TotalPawsRemain;
                updateTroopInfoInWar.TimeBattle = troop.Value.TimeBattle;
                troopList.Add(updateTroopInfoInWar);
            }

            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);

      
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                for (int j = 0; j < troopList.Count; j++)
                {
                    if (model.troopData.IdTroop == troopList[j].IdTroops)
                    {
                        int pawnsDie =troopList[j].TotalPawsRemain;
                        model.UpdatePawsDie(pawnsDie);
                    }
                }

            }
        }

        public async  void EndBattleIfNotNetwork(float timeDelay)
        {
            await Task.Delay(TimeSpan.FromSeconds(timeDelay));
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            long idPlayerLose = 1, idPlayerWin = userProfile.id;
            int idLandUserLose = 0;
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer != userProfile.id)
                {
                    idPlayerLose = model.troopData.idPlayer;
                }
                else
                {
                    idPlayerWin = model.troopData.idPlayer;
                }
            }
            EndBattleModel dataEndBattle = new EndBattleModel();
            dataEndBattle.IdLandUserWin = userProfile.landId;
            dataEndBattle.UidPlayerLose = idPlayerLose;
            dataEndBattle.UidPlayerWin = idPlayerWin;
            dataEndBattle.IdLandUserLose = idLandUserLose;
            EndBattle(dataEndBattle);
        }

        public void OnStopCourotineEndBattle()
        {
            StopCoroutine("EndBattleIfNotNetwork");
        }

        public void EndBattle(EndBattleModel dataEndBattle)
        {
            if (TutorialData.Instance.GetStepTutorial() >= TutorialData.StepTutorial.TutorialBattleBradit1 &&
                TutorialData.Instance.GetStepTutorial() < TutorialData.StepTutorial.TutorialDone)
            {
                TutorialData.Instance.SetEndBattle(dataEndBattle);
                return;
            }

            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
            {
                for (int i = 0; i < models.Length; i++)
                {
                    var model = models[i];
                    model.AnimationIdle();
                    model.OnDisableParticeBattle();
                    if (model.troopData.idPlayer == dataEndBattle.UidPlayerWin)
                    {
                        model.Win();
                        TutorialWorldmap.GetTutorialData().SetStepTutorial(TutorialData.StepTutorial.TutorialTroopWin);
                        TutorialWorldmap.OnClickShowNextTepTutorial();
                    }
                    else if(model.troopData.idPlayer == dataEndBattle.UidPlayerLose)
                    {
                        model.Lose();
                    }
                }
            }
            else
            {
                
            }
        }

        public void EndBattleShow()
        {
            WorldMapTroopModel moderEnemy = TutorialWorldmap.GetTroopEnemyTutorial();
            WorldMapTroopModel moderCavalry = TutorialWorldmap.GetTroopCavalryMeTutorial();
            WorldMapTroopModel moderTroopAi1 = TutorialWorldmap.GetTroopAIMeTutorial(1);
            WorldMapTroopModel moderTroopAi2 = TutorialWorldmap.GetTroopAIMeTutorial(2);

            moderEnemy.AnimationIdle();
            moderCavalry.AnimationIdle();
            moderTroopAi1.AnimationIdle();
            moderTroopAi2.AnimationIdle();


            var endBattle = TutorialWorldmap.GetTutorialData().GetEndBattle();
            long idPlayerWin = endBattle.UidPlayerWin;
            long idPlayerLose = endBattle.UidPlayerLose;
            int idLandWin = endBattle.IdLandUserWin;
            int idLandLose = endBattle.IdLandUserLose;
            WorldMapTroopModel[] models = new WorldMapTroopModel[troopModels.Count];
            troopModels.Values.CopyTo(models, 0);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.troopData.idPlayer == idPlayerLose)
                {
                    model.Lose();
                }
            }
            
            TutorialWorldmap.GetTutorialData().SetStepTutorial(TutorialData.StepTutorial.TutorialDone);
            TutorialDoneBattle();
        }

        private void TutorialDoneBattle()
        {

            WorldMapTroopModel moderEnemy = TutorialWorldmap.GetTroopEnemyTutorial();
            WorldMapTroopModel moderCavalry = TutorialWorldmap.GetTroopCavalryMeTutorial();
            WorldMapTroopModel moderTroopAi1 = TutorialWorldmap.GetTroopAIMeTutorial(1);
            WorldMapTroopModel moderTroopAi2 = TutorialWorldmap.GetTroopAIMeTutorial(2);

            moderCavalry.AnimationMove();
            moderTroopAi1.AnimationMove();
            moderTroopAi2.AnimationMove();
            var cityTroopEnemy = tableCityModels.GetByKey(moderEnemy.troopData.Position);


            var pointAreaTown1 = new Vector3(1018.487f, 0, 765.056f);
            var pointAreaTown2 = new Vector3(1018.356f, 0, 765.23f);
            var pointAreaTown3 = new Vector3(1018.747f, 0, 765.033f);
            moderCavalry.ChangeRotation(moderCavalry.transform, pointAreaTown1, pointAreaTown1, 200);
            moderTroopAi1.ChangeRotation(moderTroopAi1.transform, pointAreaTown2, pointAreaTown2, 200);
            moderTroopAi2.ChangeRotation(moderTroopAi2.transform, pointAreaTown3, pointAreaTown3, 200);
            
            moderCavalry.SetRotationMove(pointAreaTown1);
            moderTroopAi1.SetRotationMove(pointAreaTown2);
            moderTroopAi2.SetRotationMove(pointAreaTown3);
            moderCavalry.MoveTo(moderCavalry, pointAreaTown1, pointAreaTown1, 0.1f, async () =>
            {
                moderCavalry.ChangeRotation(moderCavalry.transform, cityTroopEnemy.Position, cityTroopEnemy.Position, 200);
                moderCavalry.AnimationAttack();
                await Task.Delay(TimeSpan.FromSeconds(10));
                moderCavalry.AnimationMove();
                moderCavalry.MoveTo(moderCavalry, cityTroopEnemy.Position, cityTroopEnemy.Position, 0.1f,
                    () =>
                    {
                        moderCavalry.AnimationIdle();
                    });

            });
            moderTroopAi1.MoveTo(moderTroopAi1, pointAreaTown2, pointAreaTown2, 0.1f, async () =>
            {
                moderTroopAi1.ChangeRotation(moderTroopAi1.transform, cityTroopEnemy.Position, cityTroopEnemy.Position, 200);
                moderTroopAi1.AnimationAttack();
                await Task.Delay(TimeSpan.FromSeconds(10));
                moderTroopAi1.AnimationMove();
                moderTroopAi1.MoveTo(moderTroopAi1, cityTroopEnemy.Position, cityTroopEnemy.Position, 0.1f,
                    () =>
                    {
                        moderTroopAi1.AnimationIdle();
                    });
            });

            moderTroopAi2.MoveTo(moderTroopAi2, pointAreaTown3, pointAreaTown3, 0.1f, async () =>
            {
                moderTroopAi2.ChangeRotation(moderTroopAi2.transform, cityTroopEnemy.Position, cityTroopEnemy.Position, 200);
                moderTroopAi2.AnimationAttack();
                await Task.Delay(TimeSpan.FromSeconds(10));
                moderTroopAi2.AnimationMove();
                moderTroopAi2.MoveTo(moderTroopAi2, cityTroopEnemy.Position, cityTroopEnemy.Position, 0.1f,
                    async () =>
                    {
                        moderTroopAi2.AnimationIdle();
                        TutorialNpc.SetPositionLandController(new Vector3(0, 0.5f, 0));
                        onZoomCamera?.Raise(moderTroopAi2, 0.42f, true);
                        var endBattle = TutorialWorldmap.GetTutorialData().GetEndBattle();
                        int idLandWin = endBattle.IdLandUserWin;
                        int idLandLose = endBattle.IdLandUserLose;
                        LandViewModel land = landAndKingdomService.GetLandById(idLandWin);
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        foreach (var View in landController.views)
                        {
                            if (View.Value.info.id == idLandWin)
                            {
                                GameObject item = Instantiate(View.Value.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                                item.transform.parent = landController.transform;
                                LandView landView = item.GetComponent<LandView>();
                                var lanviewClon = View.Value;
                                var color = View.Value.info.color;
                                lanviewClon.info.color = Color.red;
                                landView = View.Value;
                               
                                item.gameObject.name = $"land_{idLandWin.ToString("00000")}_clone";
                                item.GetComponent<Transform>().localPosition = new Vector3(View.Value.transform.position.x ,0, View.Value.transform.position.z);
                                item.GetComponent<Transform>().localRotation = Quaternion.Euler(90, 0, 0);
                                item.GetComponent<Transform>().localScale = Vector3.one;
                                item.gameObject.SetActive(true);

                                Material[] intMaterials = new Material[landView.MaterialColor.Length];
                                for (int i = 0; i < intMaterials.Length; i++)
                                {
                                    intMaterials[i] = landView.MaterialColor[i];
                                }

                                landView.meshRenderer.materials = intMaterials;
                                
                                StartCoroutine(EffectLand(View.Value, item));
                                TutorialWorldmap.GetTutorialData().TutorialTracker.SetTutorialFlag(false);
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                TutorialWorldmap.GetTutorialNpc().UiTutorialEnd.Open();
                            }
                        }
                    });
            });
     
        }

        private bool isActiveLand = false;
        public IEnumerator EffectLand(LandView landView, GameObject landViewClone)
        {
            yield return new WaitForSeconds(0.5f);
            if (isActiveLand)
            {
                isActiveLand = !isActiveLand;
                landView.gameObject.SetActive(false);
                landViewClone.SetActive(true);
            }
            else
            {
                isActiveLand = !isActiveLand;
                landView.gameObject.SetActive(true);
                landViewClone.SetActive(false);
            }
            StartCoroutine(EffectLand(landView, landViewClone));
        }

        private void OnApplicationQuit()
        {
            _ = APIManager.Instance.RequestLeaveWorldMap(StatesGlobal.UID_PLAYER);
            Destroy(this);
        }
    }

    public class UpdateTroopInfoInWarData
    {
        public int IdTroops;
        public int Damge;
        public int TotalPaws;
        public int TotalPawsRemain;
        public float TimeBattle;
    }

    [System.Serializable]
    public class EndBattleModel
    {
        public long UidPlayerWin;
        public long UidPlayerLose;
        public int IdLandUserWin;
        public int IdLandUserLose;
    }
}