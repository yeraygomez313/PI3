using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SceneManager.LoadScene("PreparationUI", LoadSceneMode.Additive);
    }

    public void StartCombat()
    {
        SceneManager.LoadScene("CombatUI", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PreparationUI");
    }

    public void CombatWon()
    {
        SceneManager.LoadScene("RewardUI", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("CombatUI");
    }

    public void CombatLost()
    {
        SceneManager.LoadScene("GameOverUI", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("CombatUI");
    }
}
