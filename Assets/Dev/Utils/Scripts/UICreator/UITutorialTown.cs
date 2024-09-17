using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData.UniFlow.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Dev.Tutorial.Scripts
{
    public class UITutorialTown : MonoBehaviour
    {
        [SerializeField] private Button[] _btnToTownButton;

        public void Start()
        {
            if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
            {
                for (int i = 0; i < _btnToTownButton.Length; i++)
                {
                    _btnToTownButton[i].enabled = false;
                }
            }
        }
    }
}
