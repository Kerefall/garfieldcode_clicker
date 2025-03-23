using UnityEngine;

public class ClickCoin : MonoBehaviour
{
    private Vector2 defaultScale;
    private Vector2 clickedScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
        clickedScale = new Vector2(defaultScale.x - .05f, defaultScale.y - .05f);
    }

    private void OnMouseDown()
    {
        transform.localScale = clickedScale;
    }

    private void OnMouseUp()
    {
        transform.localScale = defaultScale;
        Clicker.Instance.ClickedCoin();
    }
}
