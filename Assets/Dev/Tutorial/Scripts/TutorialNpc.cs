using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData.UniFlow.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Dev.Tutorial.Scripts
{
    public class TutorialNpc : MonoBehaviour
    {
        [SerializeField] private LandController landController;

        [SerializeField]
        public UIPanel UiPanelTutorialNpcStart;

        [SerializeField]
        public UIPanel UiPanelTutorialNpc;

        [SerializeField]
        public GameObject TutorialBlocker;

        [SerializeField]
        public TutorialPointer TutorialNPCPointer;

        [SerializeField]
        public GameObject TutorialNPCContent;
        
        [SerializeField]
        public UIPanel UiTutorialEnd;

        [SerializeField] private GameObject _cloudLow;

        [SerializeField] private GameObject _cloudHigh;

        [SerializeField]
        public GameObject TutorialTalkBalloonContent;

        [SerializeField] public GameObject TutorialTalkBallon;

        [SerializeField]
        public NpcTalkBalloon NpcTalkBalloon;

        [SerializeField]
        public UIPanel UiPanelTutorialNpcWarClickCity;

        [SerializeField] public GameObject TutorialBlockerClickCity;

        [SerializeField] public TutorialPointer TutorialPointerClickCity;
        
        [SerializeField]
        public UIPanel UiPanelTutorialNpcWarAttack;

        [SerializeField] public GameObject TutorialBlockerAttack;

        [SerializeField] public TutorialPointer TutorialPointerAttack;
        

        [SerializeField]
        public UIPanel[] UiPanelTutorialNpcWar;

        [SerializeField]
        public GameObject[] TutorialNPCContentWar;

        [SerializeField]
        public GameObject[] TutorialNPCTalkBallonWar;

        public LandContextMenu LandContextMenu;

        [SerializeField] private Vector3 _positionBallonNoContent;

        [SerializeField] private Vector3 _positionBallonHaveContent;

        [SerializeField] private Button _btnToTownButton;
        [SerializeField] private Button _btnBurgerMenuButton;
        [SerializeField] private Button _btnMarketButton;
        [SerializeField] private Button[] _btnCommanButton;
        public void OnDisableCloud()
        {
            _cloudLow.SetActive(false);
            _cloudHigh.SetActive(false);
        }

        public void OpenNpcWar(TutorialData.StepNpc step)
        {
            UiPanelTutorialNpcWar[(int)step].GetElement(1).gameObject.SetActive(true);
            UiPanelTutorialNpcWar[(int)step].Open();
            bool isCloseContent = true;
            switch (step)
            {
                case TutorialData.StepNpc.Npc1:
                    isCloseContent = false;
                    break;
                case TutorialData.StepNpc.Npc2:
                    break;
                case TutorialData.StepNpc.Npc3:
                    break;
                case TutorialData.StepNpc.Npc4:
                    isCloseContent = false;
                    break;
                case TutorialData.StepNpc.Npc5:
                    break;
                case TutorialData.StepNpc.Npc6:
                    break;
                case TutorialData.StepNpc.Npc7:
                    break;
                case TutorialData.StepNpc.Npc8:
                    break;
            }

            var rect = TutorialNPCTalkBallonWar[(int)step].GetComponent<RectTransform>();
            if (!isCloseContent)
            {
                TutorialNPCTalkBallonWar[(int)step].transform.localPosition = _positionBallonNoContent;
                rect.offsetMin = _positionBallonNoContent;
                rect.anchoredPosition = _positionBallonNoContent;
            }
            else
            {
                TutorialNPCTalkBallonWar[(int)step].transform.localPosition = _positionBallonHaveContent;
                rect.offsetMin = _positionBallonHaveContent;
                rect.anchoredPosition = _positionBallonHaveContent;
            }
            rect.sizeDelta = new Vector2(1700, 300);
            TutorialNPCContentWar[(int)step].SetActive(isCloseContent);
        }

        public void OpenUiPanelTutorialStart()
        {
            UiPanelTutorialNpcStart.Open();
            UiPanelTutorialNpcStart.GetElement(1).gameObject.SetActive(true);
        }

        public void CloseUiPanelTutorialStart()
        {
            UiPanelTutorialNpcStart.Close();
            UiPanelTutorialNpcStart.GetElement(1).gameObject.SetActive(false);
        }

        public void OpenUiPanelTutorialNpc()
        {
            UiPanelTutorialNpc.Open();
            TutorialNPCContent.SetActive(true);
        }

        public void NpcContentStep1(bool isActive)
        {
            var rect = TutorialTalkBallon.GetComponent<RectTransform>();
            TutorialNPCContent.SetActive(isActive);
        }

        public void CloseNpcWar(int index)
        {
            for (int i = 0; i < index; i++)
            {
                UiPanelTutorialNpcWar[i].Close();
                UiPanelTutorialNpcWar[i].GetElement(1).gameObject.SetActive(false);
            }
            UiPanelTutorialNpcWar[index].Close();
            UiPanelTutorialNpcWar[index].GetElement(1).gameObject.SetActive(false);
        }

        public void OpenNpcClickCity(Vector3 pos)
        {
            if (TutorialBlockerClickCity.GetComponent<CanvasGroup>().alpha > 0)
                return;
            OnDisableCloud();
            UiPanelTutorialNpcWarClickCity.Open();
            TutorialBlockerClickCity.GetComponent<CanvasGroup>().alpha = 1;
            var tutorialBlocker = TutorialBlockerClickCity.GetComponent<TutorialBlocker>();
            tutorialBlocker.HighlighToPosition(pos);
            TutorialPointerClickCity.gameObject.SetActive(true);
            TutorialPointerClickCity.PointToPosition(pos);
            TutorialPointerClickCity.ActivePointImage(true);
        }

        public void CloseNpcClickCity()
        {
            TutorialPointerClickCity.gameObject.SetActive(false);
            TutorialBlockerClickCity.GetComponent<CanvasGroup>().alpha = 0;
            UiPanelTutorialNpcWarClickCity.Close();
        }

        public async void OpenNpcAttack(RectTransform rect)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            UiPanelTutorialNpcWarAttack.Open();
            TutorialBlockerAttack.GetComponent<CanvasGroup>().alpha = 1;
            var tutorialBlocker = TutorialBlockerAttack.GetComponent<TutorialBlocker>();
            tutorialBlocker.HighlightUiElement(rect);
            TutorialPointerAttack.ActivePointImage(true);
        }

        public void CloseNpcAttack()
        {
            TutorialBlockerAttack.GetComponent<CanvasGroup>().alpha = 0;
            UiPanelTutorialNpcWarAttack.Close();
        }

        public void SetPositionLandController(Vector3 position)
        {
            landController.transform.position = position;
        }
        
        public void DisableSet1(bool isActive)
        {
            _btnToTownButton.enabled = isActive;
            _btnMarketButton.enabled = false;
            _btnBurgerMenuButton.enabled = false;
            for (int i = 0; i < _btnCommanButton.Length; i++)
            {
                _btnCommanButton[i].enabled = false;
            }
        }
    }
}

