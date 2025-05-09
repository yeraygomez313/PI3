using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeBar : Bar
{
    [Header("Time")]
    [SerializeField] private float maxTime = 120f;
    private float currentTime;
    [SerializeField] private TextMeshProUGUI timeText;

    [HideInInspector] public UnityEvent OnTimeUp;

    private void Awake()
    {
        currentTime = maxTime;
        UpdateMaxValue(maxTime);
        timeText.text = TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");
    }

    private void Update()
    {
        currentTime = Mathf.Clamp(currentTime - Time.deltaTime, 0, maxTime);
        UpdateBarVisuals(currentTime);
        timeText.text = TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");

        if (currentTime <= 0)
        {
            OnTimeUp?.Invoke();
        }
    }
}
