using TMPro;
using UnityEngine;

public class ManaBar : Bar
{
    [Header("Mana")]
    [SerializeField] private float maxMana = 10f;
    [SerializeField] private float initialMana = 3f;
    [SerializeField] private float manaRegenRate = 1f;
    private float currentMana;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Color fullManaColor = Color.white;

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

    public bool ConsumeMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateBarVisuals(currentMana);
            return true;
        }
        else
        {
            Debug.Log("Not enough mana!");
            return false;
        }
    }

    public bool HasEnoughMana(float amount)
    {
        return currentMana >= amount;
    }
}