using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [System.Serializable]
    public class PanelData
    {
        public Button button;   
        public GameObject panel;  
    }

    public List<PanelData> panels;

    public GameObject MainPanel;

    private void Start()
    {
        MainPanel = GameObject.Find("MainMenuPanel");
        foreach (var panelData in panels)
        {
            panelData.button.onClick.AddListener(() => TogglePanel(panelData.panel));
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (!panel.activeSelf)
        {
            foreach (var panelData in panels)
            {
                panelData.panel.SetActive(false);
            }
            panel.SetActive(true);
        }
        else
        {
            foreach (var panelData in panels)
            {
                panelData.panel.SetActive(false);
            }
            MainPanel.SetActive(true);
        }
    }
}