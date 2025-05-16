using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeBar : SelfChargingBar
{
    [Header("Time")]
    [SerializeField] private TextMeshProUGUI timeText;

    [HideInInspector] public UnityEvent OnTimeUp;

    protected override void Awake()
    {
        base.Awake();
        timeText.text = TimeSpan.FromSeconds(currentValue).ToString(@"mm\:ss");
    }

    protected override void Update()
    {
        base.Update();
        timeText.text = TimeSpan.FromSeconds(currentValue).ToString(@"mm\:ss");

        if (IsEmpty())
        {
            OnTimeUp?.Invoke();
        }
    }
}
