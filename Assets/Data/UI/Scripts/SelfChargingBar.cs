using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SelfChargingBar : Bar
{
    [Header("Self Charging Bar")]
    [field:SerializeField] public float MaxValue { get; private set; }
    [SerializeField] protected float initialValue = 0;
    protected float currentValue;
    [SerializeField] protected float valueChangeMultiplier = 1f;

    [Header("Debug")]
    [SerializeField] private bool substractAmount = false;
    [SerializeField] private float amountToSubstract = 1f;

    protected virtual void OnValidate()
    {
        if (substractAmount)
        {
            ConsumeAmount(amountToSubstract);
            substractAmount = false;
        }
    }

    protected virtual void Awake()
    {
        maxValue = MaxValue;
        initialValue = Mathf.Clamp(initialValue, 0, maxValue);
        currentValue = initialValue;
        UpdateMaxValue(maxValue);
    }

    protected virtual void Update()
    {
        currentValue = Mathf.Clamp(currentValue + valueChangeMultiplier * Time.deltaTime, 0, maxValue);
        UpdateBarVisuals(currentValue);
    }

    public virtual bool ConsumeAmount(float amount)
    {
        if (currentValue >= amount)
        {
            currentValue -= amount;
            UpdateBarVisuals(currentValue);
            return true;
        }
        return false;
    }

    public bool IsFull() => currentValue >= maxValue;
    public bool IsEmpty() => currentValue <= 0;
    public bool HasEnoughAmount(float amount) => currentValue >= amount;
}
