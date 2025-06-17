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
    }

    [Header("Settings")]
    [SerializeField] private float checkInterval = 1f;

    [Header("Achievements")]
    [SerializeField] private Achievement[] achievements;

    [Header("Notifications")]
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;
    [SerializeField] private float notificationDuration = 3f;
    [SerializeField] private float notificationSpacing = 100f;
    [SerializeField] private AudioClip notificationSound;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject mainPanel;

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
        UpdateAchievementsUI();
        StartCheckingProgress();

        if (backButton != null)
        {
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

        foreach (var ach in achievements)
        {
            if (ach.descriptionPanel != null && ach.descriptionPanel != achievement.descriptionPanel)
            {
                ach.descriptionPanel.SetActive(false);
            }
        }

        achievement.descriptionPanel.SetActive(shouldShow);
    }

    private void LoadAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Unlocked", 0) == 1;
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

    private void StartCheckingProgress()
    {
        progressCounters.Add("FraudPrevention", 0);
        progressCounters.Add("EducationPayment", 0);
        progressCounters.Add("DormPayment", 0);
        checkTimer = checkInterval;
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
        if (!IsAchievementUnlocked("FirstMoney") && Clicker.Instance.Money >= 100)
        {
            UnlockAchievement("FirstMoney");
        }

        if (!IsAchievementUnlocked("Rich") && Clicker.Instance.Money >= 100000)
        {
            UnlockAchievement("Rich");
        }
    }

    public void ReportAction(string actionType)
    {
        switch (actionType)
        {
            case "FraudPrevention":
                progressCounters["FraudPrevention"]++;
                if (progressCounters["FraudPrevention"] >= 5 && !IsAchievementUnlocked("FraudPrevention"))
                {
                    UnlockAchievement("FraudPrevention");
                }
                break;

            case "EducationPayment":
                progressCounters["EducationPayment"]++;
                if (progressCounters["EducationPayment"] >= 5 && !IsAchievementUnlocked("DiligentStudent"))
                {
                    UnlockAchievement("DiligentStudent");
                }
                break;

            case "DormPayment":
                progressCounters["DormPayment"]++;
                if (progressCounters["DormPayment"] >= 5 && !IsAchievementUnlocked("DormLife"))
                {
                    UnlockAchievement("DormLife");
                }
                break;
        }
    }

    public void UnlockAchievement(string achievementId)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.id == achievementId && !achievement.isUnlocked)
            {
                achievement.isUnlocked = true;
                PlayerPrefs.SetInt($"Achievement_{achievement.id}_Unlocked", 1);
                UpdateAchievementsUI();

                notificationQueue.Enqueue(achievement);
                if (!isShowingNotification)
                {
                    StartCoroutine(ShowNotificationQueue());
                }
                break;
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
        if (notificationPrefab == null || notificationParent == null) return;

        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        activeNotifications.Add(notification);

        RectTransform rectTransform = notification.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(0, (activeNotifications.Count - 1) * notificationSpacing);
        }

        Image icon = notification.GetComponentInChildren<Image>();
        TextMeshProUGUI[] texts = notification.GetComponentsInChildren<TextMeshProUGUI>();

        if (icon != null)
        {
            icon.sprite = achievement.isUnlocked ? achievement.icon : achievement.lockedIcon;
            icon.color = achievement.isUnlocked ? unlockedColor : lockedColor;
        }

        if (texts.Length > 0) texts[0].text = achievement.displayName ?? achievement.id;
        if (texts.Length > 1) texts[1].text = achievement.isUnlocked ?
            achievement.description ?? "Достижение разблокировано!" :
            "Новое достижение доступно!";

        if (notificationSound != null)
        {
            AudioSource.PlayClipAtPoint(notificationSound, Camera.main.transform.position);
        }

        StartCoroutine(AnimateNotification(notification, true));
        StartCoroutine(DestroyNotificationAfterDelay(notification, notificationDuration));
    }

    private IEnumerator AnimateNotification(GameObject notification, bool show)
    {
        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = notification.AddComponent<CanvasGroup>();

        float targetAlpha = show ? 1f : 0f;
        float startAlpha = show ? 0f : 1f;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator DestroyNotificationAfterDelay(GameObject notification, float delay)
    {
        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(AnimateNotification(notification, false));

        activeNotifications.Remove(notification);
        Destroy(notification);

        UpdateNotificationsPositions();
    }

    private void UpdateNotificationsPositions()
    {
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            RectTransform rt = activeNotifications[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(0, i * notificationSpacing);
            }
        }
    }

    public void ReturnToMainMenu()
    {
        foreach (var achievement in achievements)
        {
            if (achievement.descriptionPanel != null)
            {
                achievement.descriptionPanel.SetActive(false);
            }
        }

        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }
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

    [ContextMenu("Reset All Achievements")]
    public void ResetAllAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = false;
            PlayerPrefs.DeleteKey($"Achievement_{achievement.id}_Unlocked");
        }
        UpdateAchievementsUI();
        Debug.Log("All achievements reset!");
    }
}