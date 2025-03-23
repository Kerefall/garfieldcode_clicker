using TMPro;
using UnityEngine;

// Script for create some effects

public class Effect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private CanvasGroup group;

    private void Update()
    {
        group.alpha = Mathf.Lerp(group.alpha, 0, Time.deltaTime * 4);
  

        if (group.alpha < .01f)
            Destroy(gameObject);
    }

    public void SetPosition(Vector2 position)
    {
        Debug.Log("Position was created");
        transform.localPosition = position;
    }

    public void SetValue(int value)
    {
        effectText.text = "+" + value;
    }
}