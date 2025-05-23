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

    public void ListenToCombatManager(CombatManager combatManager)
    {
        combatManager.OnCardSelected.AddListener(ShowZone);
        combatManager.OnCardDeselected.AddListener(HideZone);
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
