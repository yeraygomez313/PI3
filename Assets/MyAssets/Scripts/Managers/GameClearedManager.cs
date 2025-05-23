using UnityEngine;

public class GameClearedManager : MonoBehaviour
{
    [SerializeField] private Canvas gameClearedCanvas;

    private void Awake()
    {
        gameClearedCanvas.worldCamera = Camera.main;
    }

    public void ReturnToTitleScreen()
    {
        GameManager.Instance.ReturnToTitleScreen();
    }
}
