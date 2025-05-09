using UnityEngine;

public class ButtonPlus : MonoBehaviour
{
    [SerializeField] private AudioClipPlus hoverSound;
    [SerializeField] private AudioClipPlus clickSound;

    public void PlayHoverSound() => hoverSound.PlayAtPoint(transform.position);
    public void PlayClickSound() => clickSound.PlayAtPoint(transform.position);
}
