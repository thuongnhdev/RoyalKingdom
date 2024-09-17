using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopulationDetail : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private TextMeshProUGUI tmpTitle;

    [SerializeField]
    private TextMeshProUGUI tmpCitizen;

    [SerializeField]
    private TextMeshProUGUI tmpWorker;

    [SerializeField]
    private TextMeshProUGUI tmpBirth;

    [SerializeField]
    private TextMeshProUGUI tmpEmigration;

    [SerializeField]
    private TextMeshProUGUI tmpNaturalDeath;

    [SerializeField]
    private TextMeshProUGUI tmpImmigration;

    [SerializeField]
    private TextMeshProUGUI tmpDeathInWar;

    [SerializeField]
    private GameObject popupInfo;

    [SerializeField]
    private Image imgChartIcon;

    [SerializeField]
    private GameObject panelChartLine;

    [SerializeField]
    private GameObject panelChartMap;

    public Sprite IconChartLine;
    public Sprite IconChartMap;
    public override void SetData(object[] data)
    {
        base.SetData(data);
        previousPanel = (BasePanel)data[0];
    }

    public override void Open()
    {
        base.Open();
        imgChartIcon.sprite = IconChartLine;
        panelChartLine.SetActive(true);
        panelChartMap.SetActive(false);
    }

    public override void Close()
    {
        base.Close();
        if (previousPanel != null)
        {
            previousPanel.Open();
            previousPanel = null;
        }
    }

    public void OnClickChart()
    {
        if(panelChartLine.activeInHierarchy)
        {
            imgChartIcon.sprite = IconChartMap;
            panelChartLine.SetActive(false);
            panelChartMap.SetActive(true);
        }else
        {
            imgChartIcon.sprite = IconChartLine;
            panelChartLine.SetActive(true);
            panelChartMap.SetActive(false);
        }
    }

}
