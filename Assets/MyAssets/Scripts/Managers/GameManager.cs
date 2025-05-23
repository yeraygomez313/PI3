using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameObjectList> heroFormationsPerLevel = new();
    private int maxLevel => heroFormationsPerLevel.Count - 1;
    private int currentLevel = 0;

    public GameObject GetHeroFormation()
    {
        if (currentLevel < heroFormationsPerLevel.Count)
        {
            int randomIndex = UnityEngine.Random.Range(0, heroFormationsPerLevel[currentLevel].Count);
            return heroFormationsPerLevel[currentLevel][randomIndex];
        }
        else
        {
            Debug.LogError("Current level exceeds the number of hero formations available.");
            return null;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Debug
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                SceneManager.LoadScene("PreparationUI", LoadSceneMode.Additive);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        SceneManager.UnloadSceneAsync("TitleScene");
        SceneManager.LoadScene("PreparationUI", LoadSceneMode.Additive);
    }

    public void StartCombat()
    {
        SceneManager.LoadScene("CombatUI", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PreparationUI");
        Time.timeScale = 1f;
    }

    public void CombatWon()
    {
        currentLevel++;

        if (currentLevel > maxLevel)
        {
            SceneManager.LoadScene("GameClearedUI", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("CombatUI");
        }
        else
        {
            SceneManager.LoadScene("RewardUI", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("CombatUI");
        }

        Time.timeScale = 0f;
    }

    public void CombatLost()
    {
        SceneManager.LoadScene("GameOverUI", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("CombatUI");
        Time.timeScale = 0f;
    }

    public void ReturnToTitleScreen()
    {
        SceneManager.LoadScene("TitleScene");
        SceneManager.UnloadSceneAsync("MainScene");

        if (SceneManager.GetSceneByName("RewardUI").isLoaded)
        {
            SceneManager.UnloadSceneAsync("RewardUI");
        }
        if (SceneManager.GetSceneByName("GameOverUI").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GameOverUI");
        }

        currentLevel = 0;
        Time.timeScale = 1f;
    }
}

[Serializable]
public class GameObjectList
{
    [SerializeField] private List<GameObject> objects = new();

    public GameObject this[int index]
    {
        get => objects[index];
        set => objects[index] = value;
    }

    public int Count => objects.Count;

    public void Add(GameObject obj) => objects.Add(obj);
    public void Remove(GameObject obj) => objects.Remove(obj);
    public void Clear() => objects.Clear();
}