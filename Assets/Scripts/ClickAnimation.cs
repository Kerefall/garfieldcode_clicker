using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

// Script for animation of coin's size

public class ClickAnimation : MonoBehaviour
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
        Clicker.Instance.Click();
    }
}

