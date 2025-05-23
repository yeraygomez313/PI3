using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameObjectList> heroFormationsPerLevel = new();
    private int currentLevel = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.LoadScene("PreparationUI", LoadSceneMode.Additive);

        if (heroFormationsPerLevel[currentLevel].Count == 0)
        {
            Debug.LogError("No hero formations set up in the GameManager.");
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, heroFormationsPerLevel[currentLevel].Count);
        GameObject formation = heroFormationsPerLevel[currentLevel][randomIndex];
        GameObject formationInstance = Instantiate(formation);
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