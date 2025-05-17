using UnityEngine;

public class ForbiddenZone : MonoBehaviour
{
    [SerializeField] private Color forbiddenZoneColor = new Color(1, 0, 0, 0.2f);
    [SerializeField] private Color defaultColor = new Color(1, 1, 1, 0f);
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (CombatManager.Instance == null)
        {
            Debug.LogError("CombatManager instance is null. Make sure it is initialized before using ForbiddenZone.");
            return;
        }

        CombatManager.Instance.OnCardSelected.AddListener(ShowZone);
        CombatManager.Instance.OnCardDeselected.AddListener(HideZone);
    }

    private void ShowZone(DraggableCombatCard card)
    {
        spriteRenderer.color = forbiddenZoneColor;
    }

    private void HideZone(DraggableCombatCard card)
    {
        spriteRenderer.color = defaultColor;
    }
}
