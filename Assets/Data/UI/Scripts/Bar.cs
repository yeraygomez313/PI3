using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Bar : MonoBehaviour
{
    [Header("Default")]
    [SerializeField] private Image valueBar;
    [SerializeField] private Image changeBar;
    [SerializeField] private float changeBarDelay = 0.25f;
    [SerializeField] private float changeBarDuration = 0.5f;
    private float maxValue = 10;
    private Tween changeVarValueTween;

    protected virtual void UpdateBarVisuals(float currentValue)
    {
        valueBar.fillAmount = currentValue / maxValue;

        if (changeBar == null) return;

        changeVarValueTween.Kill();

        changeBar.fillAmount = valueBar.fillAmount;

        changeVarValueTween = DOTween.To(
            () => changeBar.fillAmount,
            (x) => changeBar.fillAmount = x,
            valueBar.fillAmount,
            changeBarDuration
        ).SetDelay(changeBarDelay);
    }

    protected virtual void UpdateMaxValue(float newMaxValue)
    {
        valueBar.fillAmount /= newMaxValue / maxValue;
        maxValue = newMaxValue;

        if (changeBar == null) return;

        float elapsedTime = changeVarValueTween.Elapsed();
        changeVarValueTween.Kill();
        changeBar.fillAmount /= newMaxValue / maxValue;

        changeVarValueTween = DOTween.To(
            () => changeBar.fillAmount,
            (x) => changeBar.fillAmount = x,
            valueBar.fillAmount,
            changeBarDuration - elapsedTime
        ).SetDelay(changeBarDelay);
    }
}
