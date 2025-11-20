using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private Image imageIcon;
    [SerializeField] private Image activeIcon;
    [SerializeField] private Sprite defaultActiveState;
    [SerializeField] private Sprite readyActiveState;

    private float meterRequirement;

    public void Initialize(Sprite sprite, float requiredForActive)
    {
        imageIcon.sprite = sprite;
        meterRequirement = requiredForActive;
    }

    public void UpdateMeter(float newMeter)
    {
        activeIcon.sprite = newMeter >= meterRequirement ? readyActiveState : defaultActiveState;
    }
}