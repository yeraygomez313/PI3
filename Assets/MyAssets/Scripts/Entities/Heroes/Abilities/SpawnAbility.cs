using System.Collections;
using UnityEngine;

public class SpawnAbility : MonoBehaviour
{
    [field: SerializeField] public HabilityStats AbilityStats { get; protected set; }

    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float spawnAngle = 90f;
    [SerializeField] private bool spawnRandomly = false;
    [SerializeField] private float spawnDelay = 0.25f;

    public void ActivateAbility(Transform origin, Quaternion orientation, bool setOriginAsParent = false)
    {
        StartCoroutine(SpawnWithDelay(origin, orientation, setOriginAsParent));
    }

    private IEnumerator SpawnWithDelay(Transform origin, Quaternion orientation, bool setOriginAsParent)
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            if (setOriginAsParent)
            {
                transform.SetParent(origin);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.position = origin.position;
            }

            Vector3 spawnPosition = origin.position;
            float halfAngle = spawnAngle / 2f;

            Quaternion spawnRotation;

            if (spawnRandomly)
            {
                float randomAngle = Random.Range(-halfAngle, halfAngle);
                spawnRotation = Quaternion.Euler(0, 0, randomAngle) * orientation;
            }
            else
            {
                float angleOffset = i * (spawnAngle / (spawnCount - 1)) - halfAngle;
                spawnRotation = Quaternion.Euler(0, 0, angleOffset) * orientation;
            }

            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
            //Ability spawnedAbility = spawnedObject.GetComponent<Ability>();

            //if (spawnedAbility != null)
            //{
            //    spawnedAbility.ActivateAbility(origin, spawnRotation, setOriginAsParent);
            //}
            //else
            //{
            //    Debug.LogWarning($"Spawned object {spawnedObject.name} does not have an IAbility component.");
            //}

            yield return wait;
        }

        Destroy(gameObject);
    }
}
