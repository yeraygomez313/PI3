using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class Bar : MonoBehaviour
{
    [Header("Bar")]
    [SerializeField] protected Image valueBar;
    protected float maxValue = 10;

    protected virtual void UpdateBarVisuals(float currentValue)
    {
        valueBar.fillAmount = currentValue / maxValue;
    }

    protected virtual void UpdateMaxValue(float newMaxValue)
    {
        valueBar.fillAmount /= newMaxValue / maxValue;
        maxValue = newMaxValue;
    }
}
