using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance { get; private set; }

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string displayName;
        public string description;
        public Sprite icon;
        public Sprite lockedIcon;
        public bool isUnlocked;
        public Button achievementButton;
        public GameObject descriptionPanel;
        public int requiredProgress = 5;
        [HideInInspector] public int currentProgress;
    }

    [Header("Settings")]
    [SerializeField] private float checkInterval = 1f;

    [Header("Achievements")]
    [SerializeField] private Achievement[] achievements;

    [Header("Notifications")]
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;
    [SerializeField] private float notificationDuration = 3f;
    [SerializeField] private float notificationSpacing = 120f;
    [SerializeField] private AudioClip notificationSound;

    [Header("UI Elements")]
    [SerializeField] private Button backButton; // Кнопка "Назад"
    [SerializeField] private GameObject mainPanel; // Главная панель
    [SerializeField] private GameObject achievementsPanel; // Панель достижений

    private Dictionary<string, int> progressCounters = new Dictionary<string, int>();
    private float checkTimer;
    private Queue<Achievement> notificationQueue = new Queue<Achievement>();
    private bool isShowingNotification;
    private List<GameObject> activeNotifications = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeAchievements();
        LoadAchievements();
        InitializeCounters();
        UpdateAchievementsUI();

        // Инициализация кнопки "Назад"
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(ReturnToMainMenu);
        }
    }

    private void InitializeAchievements()
    {
        foreach (var achievement in achievements)
        {
            if (achievement.achievementButton != null)
            {
                achievement.achievementButton.onClick.RemoveAllListeners();
                achievement.achievementButton.onClick.AddListener(() => ToggleDescriptionPanel(achievement));
            }

            if (achievement.descriptionPanel != null)
            {
                achievement.descriptionPanel.SetActive(false);
            }
        }
    }

    private void ToggleDescriptionPanel(Achievement achievement)
    {
        if (achievement.descriptionPanel == null) return;

        bool shouldShow = !achievement.descriptionPanel.activeSelf;

        // Закрываем все другие панели
        foreach (var ach in achievements)
        {
            if (ach.descriptionPanel != null && ach.descriptionPanel != achievement.descriptionPanel)
            {
                ach.descriptionPanel.SetActive(false);
            }
        }

        achievement.descriptionPanel.SetActive(shouldShow);
    }

    private void InitializeCounters()
    {
        progressCounters.Add("FraudPrevention", 0);
        progressCounters.Add("EducationPayment", 0);
        progressCounters.Add("DormPayment", 0);
        checkTimer = checkInterval;
    }

    private void LoadAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Unlocked", 0) == 1;
            achievement.currentProgress = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Progress", 0);
        }
    }

    private void UpdateAchievementsUI()
    {
        foreach (var achievement in achievements)
        {
            if (achievement.achievementButton != null)
            {
                var btnImage = achievement.achievementButton.GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.sprite = achievement.isUnlocked ? achievement.icon : achievement.lockedIcon;
                }
            }
        }
    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0)
        {
            CheckAllAchievements();
            checkTimer = checkInterval;
        }
    }

    private void CheckAllAchievements()
    {
        if (!IsAchievementUnlocked("FirstMoney") && Clicker.Instance != null && Clicker.Instance.Money >= 100)
        {
            UnlockAchievement("FirstMoney");
        }

        if (!IsAchievementUnlocked("Rich") && Clicker.Instance != null && Clicker.Instance.Money >= 100000)
        {
            UnlockAchievement("Rich");
        }

        CheckProgressAchievement("FraudPrevention", "FraudPrevention");
        CheckProgressAchievement("EducationPayment", "DiligentStudent");
        CheckProgressAchievement("DormPayment", "DormLife");
    }

    private void CheckProgressAchievement(string counterKey, string achievementId)
    {
        if (progressCounters.ContainsKey(counterKey))
        {
            var achievement = GetAchievementById(achievementId);
            if (achievement != null && !achievement.isUnlocked)
            {
                achievement.currentProgress = progressCounters[counterKey];
                if (achievement.currentProgress >= achievement.requiredProgress)
                {
                    UnlockAchievement(achievementId);
                }
                PlayerPrefs.SetInt($"Achievement_{achievement.id}_Progress", achievement.currentProgress);
            }
        }
    }

    public void ReportAction(string actionType)
    {
        if (!progressCounters.ContainsKey(actionType))
        {
            Debug.LogWarning($"Counter for {actionType} not initialized!");
            return;
        }

        progressCounters[actionType]++;
        Debug.Log($"{actionType} progress: {progressCounters[actionType]}");
    }

    public void UnlockAchievement(string achievementId)
    {
        Achievement achievement = GetAchievementById(achievementId);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            PlayerPrefs.SetInt($"Achievement_{achievement.id}_Unlocked", 1);
            UpdateAchievementsUI();

            notificationQueue.Enqueue(achievement);
            if (!isShowingNotification)
            {
                StartCoroutine(ShowNotificationQueue());
            }
        }
    }

    private IEnumerator ShowNotificationQueue()
    {
        isShowingNotification = true;

        while (notificationQueue.Count > 0)
        {
            Achievement achievement = notificationQueue.Dequeue();
            ShowAchievementNotification(achievement);
            yield return new WaitForSeconds(notificationDuration);
        }

        isShowingNotification = false;
    }

    private void ShowAchievementNotification(Achievement achievement)
    {
        if (notificationPrefab == null || notificationParent == null)
        {
            Debug.LogError("Notification system not configured!");
            return;
        }

        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        activeNotifications.Add(notification);

        // Настройка RectTransform
        RectTransform rect = notification.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -30 - ((activeNotifications.Count - 1) * notificationSpacing));

        // Находим компоненты
        Image icon = notification.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null) icon.sprite = achievement.icon;

        // Проигрываем звук
        if (notificationSound != null)
        {
            AudioSource.PlayClipAtPoint(notificationSound, Camera.main.transform.position);
        }

        // Анимация
        StartCoroutine(AnimateNotification(notification, true));
        StartCoroutine(RemoveNotificationAfterDelay(notification, notificationDuration));
    }

    private IEnumerator AnimateNotification(GameObject notification, bool show)
    {
        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = notification.AddComponent<CanvasGroup>();

        float duration = 0.3f;
        float elapsed = 0f;
        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator RemoveNotificationAfterDelay(GameObject notification, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(AnimateNotification(notification, false));

        activeNotifications.Remove(notification);
        Destroy(notification);
        UpdateNotificationPositions();
    }

    private void UpdateNotificationPositions()
    {
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            RectTransform rt = activeNotifications[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(0, -30 - (i * notificationSpacing));
            }
        }
    }

    private Achievement GetAchievementById(string id)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.id == id) return achievement;
        }
        return null;
    }

    public void ReturnToMainMenu()
    {
        // Закрываем все панели описаний
        foreach (var achievement in achievements)
        {
            if (achievement.descriptionPanel != null)
            {
                achievement.descriptionPanel.SetActive(false);
            }
        }

        // Переключаем панели
        if (mainPanel != null) mainPanel.SetActive(true);
        if (achievementsPanel != null) achievementsPanel.SetActive(false);
    }

    public bool IsAchievementUnlocked(string achievementId)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.id == achievementId)
            {
                return achievement.isUnlocked;
            }
        }
        return false;
    }

    [ContextMenu("Test Notification")]
    public void TestNotification()
    {
        if (achievements.Length > 0)
        {
            UnlockAchievement(achievements[0].id);
            Debug.Log("Test notification triggered");
        }
    }

    [ContextMenu("Reset All Achievements")]
    public void ResetAllAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = false;
            achievement.currentProgress = 0;
            PlayerPrefs.DeleteKey($"Achievement_{achievement.id}_Unlocked");
            PlayerPrefs.DeleteKey($"Achievement_{achievement.id}_Progress");
        }

        progressCounters["FraudPrevention"] = 0;
        progressCounters["EducationPayment"] = 0;
        progressCounters["DormPayment"] = 0;

        UpdateAchievementsUI();
        Debug.Log("All achievements reset!");
    }
}