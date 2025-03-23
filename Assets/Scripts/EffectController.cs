using UnityEngine;

// Script for spawn effects

public class EffectController : MonoBehaviour
{
    public static EffectController Instance;
    [SerializeField] private Effect effectPref;

    private void Awake()
    {
        Instance = this;
    }

    public void SetClickEffect(int value)
    {
        Debug.Log("Set click effect");
        var pref = Instantiate(effectPref, transform, false);
        pref.SetPosition(new Vector2(400, 400));
        pref.SetValue(value);
    }

}
