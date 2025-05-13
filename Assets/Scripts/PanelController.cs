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
        public string panelName; // Добавлено для удобства отладки
    }

    [Header("Панели управления")]
    [Tooltip("Список панелей и их кнопок")]
    [SerializeField] private List<PanelData> panels = new List<PanelData>();

    [Header("Главное меню")]
    [Tooltip("Основная панель меню")]
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
    /// Проверка всех необходимых ссылок
    /// </summary>
    private void ValidateReferences()
    {
        if (panels == null || panels.Count == 0)
        {
            Debug.LogError("Panels list is not assigned or empty!", this);
            return;
        }

        bool hasErrors = false;

        // Проверка панелей и кнопок
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

        // Проверка главной панели
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
    /// Инициализация панелей и подписка на события
    /// </summary>
    private void InitializePanels()
    {
        foreach (var panelData in panels)
        {
            if (panelData == null || panelData.button == null || panelData.panel == null)
                continue;

            // Создаем локальную копию для замыкания
            GameObject panel = panelData.panel;
            panelData.button.onClick.AddListener(() => TogglePanel(panel));

            // Инициализируем состояние панели
            panel.SetActive(false);
        }

        // Активируем главное меню по умолчанию
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Переключение видимости панели
    /// </summary>
    /// <param name="panel">Панель для переключения</param>
    private void TogglePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError("Trying to toggle a null panel!", this);
            return;
        }

        bool activatePanel = !panel.activeSelf;

        // Деактивируем все панели
        SetAllPanelsInactive();

        // Активируем/деактивируем выбранную панель
        panel.SetActive(activatePanel);

        // Управление главной панелью меню
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(!activatePanel);
        }
    }

    /// <summary>
    /// Деактивирует все зарегистрированные панели
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
    /// Метод для редактора - проверяет назначенные элементы
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