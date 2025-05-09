using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : Bar
{
    [SerializeField] private GameObject intBar;

    [Header("Mana")]
    [SerializeField] private float maxMana = 10f;
    [SerializeField] private float initialMana = 3f;
    [SerializeField] private float manaRegenRate = 1f;
    private float currentMana;
    public bool HasEnoughMana(float mana) => currentMana >= mana;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Color fullManaColor = Color.white;
    [SerializeField] private GameObject manaConsumedTextPrefab;
    [SerializeField] private float manaConsumedTextDuration = 1f;

    [Header("Debug")]
    [SerializeField] private bool substractMana = false;
    [SerializeField] private float manaToSubstract = 1f;

    private void OnValidate()
    {
        if (substractMana)
        {
            Debug.Log("Substracting mana: " + manaToSubstract);
            currentMana = Mathf.Clamp(currentMana - manaToSubstract, 0, maxMana);
            UpdateBarVisuals(currentMana);
            substractMana = false;
        }
    }

    private void Awake()
    {
        currentMana = Mathf.Clamp(initialMana, 0, maxMana);
        UpdateMaxValue(maxMana);
    }

    private void Update()
    {
        currentMana = Mathf.Clamp(currentMana + manaRegenRate * Time.deltaTime, 0, maxMana);
        UpdateBarVisuals(currentMana);
        manaText.text = Mathf.Floor(currentMana).ToString();

        if (currentMana == maxMana)
        {
            manaText.color = fullManaColor;
        }
        else
        {
            manaText.color = Color.white;
        }
    }

    protected override void UpdateBarVisuals(float currentValue)
    {
        base.UpdateBarVisuals(currentValue);
        intBar.GetComponent<Image>().fillAmount = Mathf.Floor(currentValue) / maxMana;
    }

    protected override void UpdateMaxValue(float newMaxValue)
    {
        base.UpdateMaxValue(newMaxValue);
        intBar.GetComponent<Image>().fillAmount = Mathf.Floor(currentMana) / newMaxValue;
    }

    public bool ConsumeMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateBarVisuals(currentMana);

            GameObject manaConsumedTextInstance = Instantiate(manaConsumedTextPrefab, manaText.transform);
            RectTransform manaConsumedTextRect = manaConsumedTextInstance.GetComponent<RectTransform>();
            manaConsumedTextRect.anchoredPosition = Vector2.zero; // or localPosition = Vector3.zero;

            manaConsumedTextInstance.GetComponent<TextMeshProUGUI>().text = "-" + amount.ToString();

            manaConsumedTextRect.DOLocalMoveX(100f, manaConsumedTextDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => Destroy(manaConsumedTextInstance));

            manaConsumedTextInstance.GetComponent<TextMeshProUGUI>().DOFade(0f, manaConsumedTextDuration)
                .SetEase(Ease.OutCubic);

            return true;
        }
        else
        {
            Debug.Log("Not enough mana!");
            return false;
        }
    }
}