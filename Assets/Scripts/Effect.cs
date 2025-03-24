using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// Script for create some effects

public class Effect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private CanvasGroup group;
    
    private Vector3 randomDirection;
    private float speedAnimation;

    private void Awake()
    {
        speedAnimation = 690f;
        randomDirection = UnityEngine.Random.insideUnitSphere.normalized * speedAnimation;
    }

    private void Update()
    {
        group.alpha = Mathf.Lerp(group.alpha, 0, Time.deltaTime * 4);
        transform.localPosition += randomDirection * Time.deltaTime;
        if (group.alpha < .01f)
            Destroy(gameObject);
    }

    public void SetValue(int value)
    {
        transform.localPosition = new Vector2(0.15f, 0.04f); // Position of coin
        effectText.text = "+" + value;
    }
}