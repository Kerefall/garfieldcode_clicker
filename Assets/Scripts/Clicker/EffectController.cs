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
        var pref = Instantiate(effectPref, transform, false);
        pref.SetValue(value);
    }

}
