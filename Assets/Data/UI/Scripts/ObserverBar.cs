using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObserverBar : Bar
{
    [Header("Observer Bar")]
    [SerializeField] protected Image changeBar;
    [SerializeField] private float changeBarDelay = 0.25f;
    [SerializeField] private float changeBarDuration = 0.5f;
    private Tween changeVarValueTween;

    private UnityEvent<float> currentValueChangedEvent;
    private UnityEvent<float> maxValueChangedEvent;

    public void SetSubject(float maxValue, UnityEvent<float> currentValueChangedEvent, UnityEvent<float> maxValueChangedEvent)
    {
        this.maxValue = maxValue;
        UpdateMaxValue(maxValue);
        this.currentValueChangedEvent = currentValueChangedEvent;
        currentValueChangedEvent.AddListener(UpdateBarVisuals);
        this.maxValueChangedEvent = maxValueChangedEvent;
        maxValueChangedEvent.AddListener(UpdateMaxValue);
    }

    protected virtual void OnDisable()
    {
        currentValueChangedEvent.RemoveListener(UpdateBarVisuals);
        maxValueChangedEvent.RemoveListener(UpdateMaxValue);
    }

    protected override void UpdateBarVisuals(float currentValue)
    {
        base.UpdateBarVisuals(currentValue);

        changeVarValueTween.Complete();
        changeBar.fillAmount = currentValue / maxValue;

        changeVarValueTween = DOTween.To(
            () => changeBar.fillAmount,
            (x) => changeBar.fillAmount = x,
            valueBar.fillAmount,
            changeBarDuration
        ).SetDelay(changeBarDelay);
    }

    protected override void UpdateMaxValue(float newMaxValue)
    {
        base.UpdateMaxValue(newMaxValue);

        if (!changeVarValueTween.IsActive())
        {
            changeBar.fillAmount = valueBar.fillAmount;
            return;
        }

        float elapsedTime = changeVarValueTween.Elapsed();
        changeVarValueTween.Complete();
        changeBar.fillAmount /= newMaxValue / maxValue;

        changeVarValueTween = DOTween.To(
            () => changeBar.fillAmount,
            (x) => changeBar.fillAmount = x,
            valueBar.fillAmount,
            changeBarDuration - elapsedTime
        ).SetDelay(changeBarDelay);
    }
}
