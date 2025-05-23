using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;

    private void Awake()
    {
        gameOverCanvas.worldCamera = Camera.main;
    }

    public void ReturnToTitleScreen()
    {
        GameManager.Instance.ReturnToTitleScreen();
    }
}
