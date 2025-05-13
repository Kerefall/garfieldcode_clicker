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
        public string panelName; // ��������� ��� �������� �������
    }

    [Header("������ ����������")]
    [Tooltip("������ ������� � �� ������")]
    [SerializeField] private List<PanelData> panels = new List<PanelData>();

    [Header("������� ����")]
    [Tooltip("�������� ������ ����")]
    [SerializeField] private GameObject mainMenuPanel;

    private void Awake()
    {
        ValidateReferences();
    }

    private void Start()
    {
        InitializePanels();
    }

    /// <summary>
    /// �������� ���� ����������� ������
    /// </summary>
    private void ValidateReferences()
    {
        if (panels == null || panels.Count == 0)
        {
            Debug.LogError("Panels list is not assigned or empty!", this);
            return;
        }

        bool hasErrors = false;

        // �������� ������� � ������
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i] == null)
            {
                Debug.LogError($"PanelData at index {i} is null!", this);
                hasErrors = true;
                continue;
            }

            if (panels[i].button == null)
            {
                Debug.LogError($"Button is not assigned for panel at index {i}!", this);
                hasErrors = true;
            }

            if (panels[i].panel == null)
            {
                Debug.LogError($"Panel is not assigned for panel at index {i}!", this);
                hasErrors = true;
            }
        }

        // �������� ������� ������
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("MainMenuPanel is not assigned, trying to find...", this);
            mainMenuPanel = GameObject.Find("MainMenuPanel");

            if (mainMenuPanel == null)
            {
                Debug.LogError("MainMenuPanel not found in scene!", this);
                hasErrors = true;
            }
        }

        if (hasErrors)
        {
            Debug.LogError("PanelController has initialization errors!", this);
        }
    }

    /// <summary>
    /// ������������� ������� � �������� �� �������
    /// </summary>
    private void InitializePanels()
    {
        foreach (var panelData in panels)
        {
            if (panelData == null || panelData.button == null || panelData.panel == null)
                continue;

            // ������� ��������� ����� ��� ���������
            GameObject panel = panelData.panel;
            panelData.button.onClick.AddListener(() => TogglePanel(panel));

            // �������������� ��������� ������
            panel.SetActive(false);
        }

        // ���������� ������� ���� �� ���������
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    /// <summary>
    /// ������������ ��������� ������
    /// </summary>
    /// <param name="panel">������ ��� ������������</param>
    private void TogglePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError("Trying to toggle a null panel!", this);
            return;
        }

        bool activatePanel = !panel.activeSelf;

        // ������������ ��� ������
        SetAllPanelsInactive();

        // ����������/������������ ��������� ������
        panel.SetActive(activatePanel);

        // ���������� ������� ������� ����
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(!activatePanel);
        }
    }

    /// <summary>
    /// ������������ ��� ������������������ ������
    /// </summary>
    private void SetAllPanelsInactive()
    {
        foreach (var panelData in panels)
        {
            if (panelData != null && panelData.panel != null)
                panelData.panel.SetActive(false);
        }
    }

    /// <summary>
    /// ����� ��� ��������� - ��������� ����������� ��������
    /// </summary>
    [ContextMenu("Validate Panel Assignments")]
    private void ValidatePanelAssignments()
    {
        if (panels == null || panels.Count == 0)
        {
            Debug.LogError("No panels assigned!", this);
            return;
        }

        for (int i = 0; i < panels.Count; i++)
        {
            string status = $"Panel {i}: ";
            status += panels[i] == null ? "NULL ENTRY" :
                     (panels[i].panel == null ? "MISSING PANEL" : $"'{panels[i].panel.name}'");
            status += " | ";
            status += panels[i] == null ? "NO BUTTON" :
                     (panels[i].button == null ? "MISSING BUTTON" : $"'{panels[i].button.name}'");

            if (panels[i] == null || panels[i].panel == null || panels[i].button == null)
            {
                Debug.LogError(status, this);
            }
            else
            {
                Debug.Log(status, this);
            }
        }

        Debug.Log(mainMenuPanel == null ?
            "MainMenuPanel is NOT assigned!" :
            $"MainMenuPanel assigned: '{mainMenuPanel.name}'", this);
    }
}