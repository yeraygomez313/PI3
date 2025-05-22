using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PreparationMode : MonoBehaviour
{

  
    public static PreparationMode Instance { get; private set; }

    private PreparationCard[] cardPlayerList = new PreparationCard[5];


    [SerializeField] private GameObject preparationUI;
    [SerializeField] private CanvasGroup startCombatButton;
    //[SerializeField] private int roundcount = 0;


    public bool CanPlayerPlaceMoreCards => cardPlayerList.Count(element => element != null) < 5;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (CombatManager.Instance == null)
        {
            Debug.LogError("CombatManager instance is null. Check if this is because you are in the preparation scene.");
            return;
        }

    }

    public void CheckList()
    {

    }

}
