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
    [SerializeField] private GameObject mainMenuPanel; // Изменил тип на GameObject

    private void Start()
    {
        // Если панель не назначена в инспекторе, пытаемся найти
        if (mainMenuPanel == null)
        {
            mainMenuPanel = GameObject.Find("MainMenuPanel");
            if (mainMenuPanel == null)
            {
                Debug.LogError("MainMenuPanel not found in scene!");
            }
        }

        foreach (var panelData in panels)
        {
            if (panelData.button == null || panelData.panel == null)
            {
                Debug.LogError("PanelData elements are not properly assigned!");
                continue;
            }

            var currentPanel = panelData.panel;
            panelData.button.onClick.AddListener(() => TogglePanel(currentPanel));
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError("Trying to toggle a null panel!");
            return;
        }

        if (!panel.activeSelf)
        {
            SetAllPanelsInactive();
            panel.SetActive(true);

            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
        }
        else
        {
            SetAllPanelsInactive();
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
        }
    }

    private void SetAllPanelsInactive()
    {
        foreach (var panelData in panels)
        {
            if (panelData.panel != null)
                panelData.panel.SetActive(false);
        }
    }
}