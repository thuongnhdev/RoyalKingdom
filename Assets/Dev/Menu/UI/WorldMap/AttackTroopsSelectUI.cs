using System.Collections;
using System.Collections.Generic;
using Assets.Dev.Tutorial.Scripts;
using UnityEngine;
using CoreData.UniFlow.Common;

public class AttackTroopsSelectUI : MonoBehaviour
{
    public WMTroopView[] viewList;

    public WorldMapTroopService troopService;
    public IntegerVariable selectedCity;
    public UserProfile userProfile;

    WorldMapPlayer player;

    public UIPanel UiPanel;
    public void InitView() {
        var troops = troopService.findByPlayerAndType(userProfile.id, 0);
        for (int i = 0; i < viewList.Length; i++)
        {
            var view = viewList[i];
            view.Selected = false;
            if (i >= troops.Count)
            {
                view.gameObject.SetActive(false);
            }
            else
            {
                var troop = troops[i];
                view.SetModel(troop);
                if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
                {
                    view.Selected = true;
                }
                else
                {
                    view.gameObject.SetActive(true);
                }
            }
        }
    }


    // Update is called once per frame
    void Update() {
    }

    public void Confirm() {
        List<WorldMapTroop> selectedTroops = new List<WorldMapTroop>();
        foreach (var view in viewList)
        {
            if (view.Selected) {
                selectedTroops.Add(view.data);
            }
        }

        var error = troopService.MoveToCity(selectedTroops, selectedCity.Value);
        if (error.Count > 0)
        {
            foreach (var t in error) {
                Debug.Log($"Can move {t.IdTroop}" );
            }
        }
    }


}
