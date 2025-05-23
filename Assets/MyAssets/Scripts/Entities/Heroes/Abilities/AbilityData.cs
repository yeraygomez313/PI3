using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AbilityData", menuName = "PI3/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string Name => name;
    public string Description = "";
    public Sprite Icon;
    public float Cooldown = 10;
    [Space]
    public AssetReferenceGameObject abilityInstancePrefab;
    public GameObject AreaPrefab;
    public float Scale = 1; // Still needs to be applied to the VFX
    public float Duration;
    public float TickRate;
    public bool LinkedToCaster;
    public float ForwardOffset;
    [Space]
    [SerializeReference, SubclassSelector] public List<AbilityEffect> OnAwakeEffects = new();
    [SerializeReference, SubclassSelector] public List<AbilityEffect> OnEntityContactEffects = new();

    public void InstantiateAbility(LivingEntity caster, Quaternion orientation)
    {
        AbilityInstance abilityInstance = Object.Instantiate(abilityInstancePrefab.Asset, caster.transform.position, orientation).GetComponent<AbilityInstance>();
        abilityInstance.Initialize(this, caster, orientation);
    }
}
