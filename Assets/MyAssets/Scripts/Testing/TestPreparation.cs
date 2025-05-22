using UnityEngine;

public class TestPreparation : MonoBehaviour
{
    public void StartCombat()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager != null)
        {
            gameManager.StartCombat();
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }
}
