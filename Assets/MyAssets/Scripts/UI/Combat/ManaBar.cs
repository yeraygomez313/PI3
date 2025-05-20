using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : SelfChargingBar
{
    [Header("Mana")]
    [SerializeField] private GameObject intBar;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Color fullManaColor = Color.white;
    [SerializeField] private GameObject manaConsumedTextPrefab;
    [SerializeField] private float manaConsumedTextDuration = 1f;

    protected override void Update()
    {
        base.Update();
        manaText.text = Mathf.Floor(currentValue).ToString();
        manaText.color = IsFull() ? fullManaColor : Color.white;
    }

    protected override void UpdateBarVisuals(float currentValue)
    {
        base.UpdateBarVisuals(currentValue);
        intBar.GetComponent<Image>().fillAmount = Mathf.Floor(currentValue) / maxValue;
    }

    protected override void UpdateMaxValue(float newMaxValue)
    {
        base.UpdateMaxValue(newMaxValue);
        intBar.GetComponent<Image>().fillAmount = Mathf.Floor(currentValue) / newMaxValue;
    }

    public override bool ConsumeAmount(float amount)
    {
        amount = Mathf.Floor(amount);

        bool success = base.ConsumeAmount(amount);

        if (success)
        {
            GameObject manaConsumedTextInstance = Instantiate(manaConsumedTextPrefab, manaText.transform);
            RectTransform manaConsumedTextRect = manaConsumedTextInstance.GetComponent<RectTransform>();
            manaConsumedTextRect.anchoredPosition = Vector2.zero; // or localPosition = Vector3.zero;

            manaConsumedTextInstance.GetComponent<TextMeshProUGUI>().text = "-" + amount.ToString();

            manaConsumedTextRect.DOLocalMoveX(100f, manaConsumedTextDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => Destroy(manaConsumedTextInstance));

            manaConsumedTextInstance.GetComponent<TextMeshProUGUI>().DOFade(0f, manaConsumedTextDuration)
                .SetEase(Ease.OutCubic);
        }

        return success;
    }
}